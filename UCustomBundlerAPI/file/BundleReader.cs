using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using ScottyFoxArt.UnityBundler.Json;
using System.IO.Pipes;
namespace ScottyFoxArt.UnityBundler.Files
{
    public static class BundleReader
    {
        public static List<UnityBundle_Data> Fetch_UnityBundles_At_Directory(string path)
        {
            List<UnityBundle_Data> results = new();
            if (!Directory.Exists(path))
            {
                Debug.Log($"Invalid Directory Cannot Search For UnityBundles -> {path}");
                return results;
            }
            var compressed_Paths = Search_Directory_For_Tagged_Files(path, UnityBundler.BUNDLE_DATA_COMPRESSED_BUNDLE_TAG);
            var assetbundle_Paths = Search_Directory_For_Tagged_Files(path, UnityBundler.BUNDLE_DATA_COMPRESSED_BUNDLE_TAG);
            var bundledata_Paths = Search_Directory_For_Files(path, UnityBundler.BUNDLE_DATA_FILENAME);
            var compressed_Bundles = Load_Compressed_UnityBundles(compressed_Paths);
            var assetbundle_Bundles = Load_AssetBundle_UnityBundles(compressed_Paths);
            var bundledata_Bundles = Load_NonCompressed_UnityBundles(compressed_Paths);
            results.AddRange(compressed_Bundles);
            results.AddRange(assetbundle_Bundles);
            results.AddRange(bundledata_Bundles);
            Debug.Log($"Total Of {results.Count} UnityBundles Found At \"{path}\"");
            return results;
        }
        public static List<UnityBundle_Data> Load_Compressed_UnityBundles(List<string> paths)
        {
            var results = new List<UnityBundle_Data>();
            foreach (var path in paths)
            {
                try
                {
                    using (FileStream stream = File.OpenRead(path))
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);
                            stream.Close();
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            Debug.Log($"Loading Compressed Bundle -> \"{path}\"");
                            if (TryLoad_Compressed_UnityBundle(memoryStream, out var bundle))
                                results.Add(bundle);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Unable To Read File \"{path}\"");
                    Debug.LogError(e);
                }
            }
            return results;
        }
        public static List<UnityBundle_Data> Load_AssetBundle_UnityBundles(List<string> paths)
        {
            var results = new List<UnityBundle_Data>();
            foreach (var path in paths)
            {
                try
                {
                    using (FileStream stream = File.OpenRead(path))
                    {
                        Debug.Log($"Loading Assetbundle Bundle -> \"{path}\"");
                        if (TryLoad_AssetBundle_UnityBundle(stream, out var bundle))
                            results.Add(bundle);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Unable To Read File \"{path}\"");
                    Debug.LogError(e);
                }
            }
            return results;
        }
        public static List<UnityBundle_Data> Load_NonCompressed_UnityBundles(List<string> paths)
        {
            var results = new List<UnityBundle_Data>();
            foreach (var path in paths)
            {
                try
                {
                    Debug.Log($"Loading NonCompressed Bundle -> \"{path}\"");
                    if (TryLoad_NonCompressed_UnityBundle(path, out var bundle))
                        results.Add(bundle);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Unable To Read File \"{path}\"");
                    Debug.LogError(e);
                }
            }
            return results;
        }
        private static List<string> Search_Directory_For_Files(string path, string name)
        {
            return new List<string>(Directory.GetFiles(path, name, SearchOption.AllDirectories));
        }
        private static List<string> Search_Directory_For_Tagged_Files(string path, string tag)
        {
            var searchResults = Directory.GetFiles(path, "*", SearchOption.AllDirectories)
                               .Where(e => e.EndsWith(tag));
            return new List<string>(searchResults);
        }
        public class UnityBundle_Data
        {
            public BundleInfo info;
            public Dictionary<string, object> data = new Dictionary<string, object>();
        }
        public static bool TryLoad_Compressed_UnityBundle(Stream stream, out UnityBundle_Data bundle)
        {
            bundle = new UnityBundle_Data();
            try
            {
                using (ZipArchive archive = new(stream, ZipArchiveMode.Read))
                {
                    if (!TryRead_Archived_BundleInfo(archive, out bundle.info))
                    {
                        Debug.LogWarning($"UnityBundle Unable To Read Data File. \"{UnityBundler.BUNDLE_DATA_FILENAME}\"");
                        return false;
                    }
                    foreach (var asset_infos in bundle.info.assets)
                    {
                        var assetbundle_entry = archive.Entries.FirstOrDefault(e => e.Name == asset_infos.assetBundle);
                        Debug.Log(assetbundle_entry.FullName);
                        if (assetbundle_entry == null)
                        {
                            Debug.LogWarning($"UnityBundle Unable to Find Assetbundle \"{asset_infos.assetBundle}\"");
                            continue;
                        }
                        if (!TryRead_AssetBundle(assetbundle_entry.Open(), out var assetbundle))
                        {
                            Debug.LogWarning($"UnityBundle Unable to Read Assetbundle \"{asset_infos.assetBundle}\"");
                            continue;
                        }
                        //Adding Assetbundle To Data.
                        bundle.data[$"{asset_infos.assetBundle}_assetbundle"] = assetbundle;
                        foreach (var assetinfo in asset_infos.manifest)
                        {
                            var tag = assetinfo.type.Substring(0, assetinfo.type.IndexOf('|') + 1);
                            Debug.Log($"Checking Tag {tag}");
                            switch (tag)
                            {
                                case UnityBundler.JSON_TYPE_TAG:
                                    assetinfo.AssetType = BundleAssetType.JSON;
                                    assetinfo.type = assetinfo.type.Substring(tag.Length);
                                    Debug.Log($"Checking Type {assetinfo.type}");
                                    var entry = archive.GetEntry(assetinfo.path + ".json");
                                    if (entry != null)
                                    {
                                        TryRead_TxtFile(entry.Open(), out var json);
                                        bundle.data[assetinfo.path] = json;
                                    }
                                    else
                                    {
                                        foreach (var aentry in archive.Entries)
                                            Debug.Log(aentry.FullName + ": " + aentry.Name);
                                    }
                                    break;
                                default:
                                    assetinfo.AssetType = BundleAssetType.Asset;
                                    object asset = null;
                                    {
                                        {
                                            Debug.Log($"Finding Asset -> \"{assetinfo.path}\"");
                                            asset = assetbundle.LoadAsset(assetinfo.path);
                                        }
                                        if (asset == null)
                                        {
                                            Debug.Log($"Checking ID -> \"{assetinfo.path}\"");
                                            asset = assetbundle.LoadAsset(assetinfo.id);
                                        }
                                        if (asset != null)
                                        {
                                            Debug.Log($"Found Asset -> \"{assetinfo.id}\"");
                                            bundle.data[assetinfo.path] = asset;
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Unable To Load Compressed UnityBundle");
                Debug.LogError(e);
                return false;
            }
            return true;
        }
        public static bool TryLoad_NonCompressed_UnityBundle(string path, out UnityBundle_Data bundle)
        {
            bundle = new UnityBundle_Data();
            Debug.LogWarning($"Uncompressed UnityBundle Not Supported Yet");
            return false;
        }
        public static bool TryLoad_AssetBundle_UnityBundle(Stream stream, out UnityBundle_Data bundle)
        {
            bundle = new UnityBundle_Data();
            Debug.LogWarning($"Assetbundle UnityBundle Not Supported Yet");
            return false;
        }
        private static bool TryRead_BundleInfo(Stream stream, out BundleInfo info)
        {
            info = null;
            if (!TryRead_TxtFile(stream, out var jsonTxt))
            {
                return false;
            }
            try
            {
                info = JsonConvert.DeserializeObject<BundleInfo>(jsonTxt);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Error Deserializing Json \"{UnityBundler.BUNDLE_DATA_FILENAME}\"");
                Debug.LogError(e);
                return false;
            }
        }
        private static bool TryRead_Archived_BundleInfo(ZipArchive archive, out BundleInfo info)
        {
            info = null;
            var bundledata_entry = archive.Entries.FirstOrDefault(e => e.Name == UnityBundler.BUNDLE_DATA_FILENAME);
            if (bundledata_entry == null)
            {
                Debug.LogWarning($"UnityBundle Does Not Contain Data File \"{UnityBundler.BUNDLE_DATA_FILENAME}\"");
                return false;
            }
            return TryRead_BundleInfo(bundledata_entry.Open(), out info);
        }
        private static bool TryRead_TxtFile(Stream stream, out string txtFile)
        {
            txtFile = string.Empty;
            using (StreamReader reader = new StreamReader(stream))
            {
                try
                {
                    txtFile = reader.ReadToEnd();
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Error Reading Text File");
                    Debug.LogError(e);
                    return false;
                }
            }
        }
        private static bool TryRead_AssetBundle(Stream stream, out AssetBundle assetbundle)
        {
            assetbundle = null;
            if (!stream.CanRead || !stream.CanSeek)
            {
                using (MemoryStream memoryStream = new())
                {
                    stream.CopyTo(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    stream.Close();
                    stream = memoryStream;
                }
            }
            try
            {
                assetbundle = AssetBundle.LoadFromStream(stream);
                if (assetbundle != null)
                    return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error Reading Assetbundle.");
                Debug.LogError(e);
            }
            return false;
        }

    }
}