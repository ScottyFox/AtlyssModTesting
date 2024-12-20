﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace AtlyssHelperUtils.Files
{
    public static class FileHelpers
    {
        public static List<string> FindDirectoriesWithFileName(string path, string fileName)
        {
            List<string> results = new List<string>();
            Queue<string> workingDirectories = new Queue<string>();
            workingDirectories.Enqueue(path);
            while (workingDirectories.Count > 0)
            {
                string directory = workingDirectories.Dequeue();
                try
                {
                    if (File.Exists(Path.Combine(directory, fileName)))
                        results.Add(directory);
                    foreach (string subDirectory in Directory.GetDirectories(directory))
                        workingDirectories.Enqueue(subDirectory);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Unable to Access {directory}");
                    Debug.LogError(e);
                }
            }
            return results;
        }
        private static bool IsAssetBundle(string path)
        {
            bool valid = false;
            try
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader reader = new BinaryReader(fileStream))
                    {
                        byte[] headerBytes = reader.ReadBytes(8);
                        string header = System.Text.Encoding.ASCII.GetString(headerBytes);
                        if (header.StartsWith("UnityFS") ||
                            header.StartsWith("UnityRaw") ||
                            header.StartsWith("UnityWeb"))
                            valid = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Reading File : {ex.Message}");
            }
            return valid;
        }
        /*public static void TryToLoadTemplateAssetBundlesFromPath<T>(string path)
        {
            try
            {
                var assemblyPath = Path.GetDirectoryName(typeof(T).Assembly.Location);
                var fullPath = Path.Combine(assemblyPath, path);
                var files = Directory.GetFiles(fullPath);
                var ValidAssetBundlePaths = new List<string>();
                foreach (var file in files)
                {
                    if (IsAssetBundle(file))
                    {
                        ValidAssetBundlePaths.Add(Path.Combine(path, Path.GetFileName(file)));
                    }
                }
                foreach (var assetbundlePath in ValidAssetBundlePaths)
                {
                    if (AssetBundleRegistry.Register<T>(assetbundlePath, out var name))
                        if (!TryToLoadTemplatesFromAssetBundle(name))
                        {
                            Debug.Log($"Unable to load Templates from Assetbundle : {name}");
                            AssetBundleRegistry.Remove(name, true);
                        }
                        else
                            Debug.Log($"Unable to Register AssetBundle : {assetbundlePath}");
                }
            }
            catch (Exception e)
            {
                Debug.Log($"Unable To Access Path : {path}");
                Debug.LogError(e);
            }
        }
        public static bool TryToLoadTemplatesFromAssetBundle(string assetBundleName)
        {
            var templatesJson = AssetBundleRegistry.LoadAsset<TextAsset>(assetBundleName, "templates.ucp.txt");
            if (templatesJson == null || string.IsNullOrWhiteSpace(templatesJson.text))
            {
                Debug.Log($"Unable to Read Templates Json from {assetBundleName}");
                return false;
            }
            var data = new UCustomPrefab_AssetBundle_TemplatesJSON();
            JsonUtility.FromJsonOverwrite(templatesJson.text, data);
            if (!data.Verify())
            {
                Debug.Log("Templates Data is Invalid!");
                return false;
            }
            for (int i = 0; i < data.Template_Names.Count; i++)
            {
                var templatePrefab = AssetBundleRegistry.LoadPrefab(assetBundleName, data.Template_Prefabs[i]);
                if (templatePrefab == null)
                    continue;
                TemplateRegistry.Register(data.Template_Names[i], templatePrefab);
            }
            return true;
        }*/
    }
}