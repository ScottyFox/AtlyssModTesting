using ScottyFoxArt.UnityBundler.Files;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ScottyFoxArt.UnityBundler.Json;
using ScottyFoxArt.UnityBundler.Utilities;
using System.Runtime.InteropServices;
namespace ScottyFoxArt.UnityBundler
{
    public static class BundleBuilder
    {
        public static Dictionary<string, BundleReader.UnityBundle_Data> BundleInfo_Data = new();
        private static HashSet<GameObject> _cached_assets = new();
        private static GameObject _cached_asset_Container;
        public static GameObject _Cached_Asset_Container
        {
            get
            {
                if (_cached_asset_Container == null)
                {
                    _cached_asset_Container = new GameObject("UnityBundle_Cached_Assets");
                    GameObject.DontDestroyOnLoad(_cached_asset_Container);
                    _cached_asset_Container.hideFlags = HideFlags.HideAndDontSave;
                    _cached_asset_Container.SetActive(false);
                }
                return _cached_asset_Container;
            }
        }
        public static GameObject Cache_GameObject_Asset(GameObject asset)
        {
            GameObject instance = null;
            try
            {
                instance = GameObject.Instantiate(asset, _Cached_Asset_Container.transform);
                if (instance != null)
                    _cached_assets.Add(instance);
            }
            catch (System.Exception e)
            {
                Debug.Log("Unable To Instantiate Asset.");
                Debug.LogError(e.Message);
            }
            return instance;
        }
        public static void Handle_ComponentJsonSerializers(GameObject target)
        {
            HashSet<ComponentJsonSerializer> components = new();
            target.GetComponents<ComponentJsonSerializer>().All(components.Add);
            target.GetComponentsInChildren<ComponentJsonSerializer>().All(components.Add);
            foreach (var component in components)
            {
                component.DeserializeJsonInfo();
            }
        }
        private static void Register_Asset(string type, BundleAssetInfo info, UnityEngine.Object asset)
        {
            if (BundleRegistry.TryGet_TypeRegisterer(type, out var registerer))
            {
                Debug.Log($"Registering Asset ID \"{info.id}\" as \"{type}\"");
                registerer(asset);
            }
            else
                Debug.LogWarning($"No Registerer -> {type}");
        }
        public static void Register(BundleReader.UnityBundle_Data bundle_Data)
        {
            BundleInfo_Data.Add(bundle_Data.info.Name, bundle_Data);
        }
        public static void Finallize()
        {
            Debug.Log("Registering Assets");
            foreach (var pair in BundleInfo_Data)
            {
                Register_BundleAssets(pair.Value);
            }
            Debug.Log("Finallizing Assets");
            foreach (var pair in BundleInfo_Data)
            {
                Finallize_BundleAssets(pair.Value);
            }
            Debug.Log("Checking GameObject Utilities");
            //Special Utility Checks
            foreach (var obj in _cached_assets)
            {
                Handle_ComponentJsonSerializers(obj);
            }
            Debug.Log("Unloading Assetbundles");
            //Unload From Memory
            foreach (var pair in BundleInfo_Data)
            {
                var data = pair.Value.data;
                foreach (var data_pair in data)
                {
                    if (data_pair.Value is AssetBundle assetbundle)
                        assetbundle.UnloadAsync(false);
                }
            }
            Debug.Log("Clearing Working Data");
            BundleInfo_Data.Clear();
            Debug.Log("Done!");
        }
        private static void Register_BundleAssets(BundleReader.UnityBundle_Data bundle_Data)
        {
            foreach (var assets in bundle_Data.info.assets)
            {
                var assetbundle_Name = assets.assetBundle;
                if (!bundle_Data.data.TryGetValue(assetbundle_Name + "_assetbundle", out var assetbundle_obj))
                    continue;
                var assetbundle = (AssetBundle)assetbundle_obj;

                foreach (var assetInfo in assets.manifest)
                {
                    var useRegistry = false;
                    UnityEngine.Object asset = null; ;
                    switch (assetInfo.AssetType)
                    {
                        case BundleAssetType.JSON:
                            Debug.Log($"Checking Type : {assetInfo.id} -> {assetInfo.type}");
                            var type = ReflectionUtils.Fetch_Type_By_Name(assetInfo.type);
                            if (type == null)
                                continue;
                            Debug.Log($"Type Valid");
                            asset = (UnityEngine.Object)ReflectionUtils.Create_Instance(type);
                            break;
                        default:
                            asset = Dirty_FetchAsset_Via_Info(assetInfo, assetbundle);
                            useRegistry = true;
                            break;
                    }
                    var assetType = assetInfo.type;
                    if (asset is GameObject gObj)
                    {
                        asset = Cache_GameObject_Asset(gObj);
                    }
                    if (asset == null)
                    {
                        Debug.Log($"Invalid Asset In {bundle_Data.info.Name} : {assetInfo.id}");
                        continue;
                    }
                    Debug.Log($"Registering Asset In {bundle_Data.info.Name} : {assetInfo.id}");
                    BundleRegistry.Register(bundle_Data.info.Name, assetInfo.id, asset);
                    BundleRegistry.RegisterPath(bundle_Data.info.Name, assetInfo.path, assetInfo.id);
                    //Backup Registering For Assetbundle assets that need no special creation.
                    if (useRegistry)
                    {
                        Register_Asset(assetType, assetInfo, asset);
                    }
                }
            }
        }
        //TODO Weird Behaviours with fetching assets via Pathing for some reason, Remove this later and use Addressables.
        private static UnityEngine.Object Dirty_FetchAsset_Via_Info(BundleAssetInfo assetInfo, AssetBundle assetbundle)
        {
            if (assetbundle == null)
                return null;
            UnityEngine.Object asset = assetbundle.LoadAsset(assetInfo.path);
            if (asset == null)
                asset = assetbundle.LoadAsset(assetInfo.id);
            if (asset == null)
                asset = assetbundle.LoadAllAssets().FirstOrDefault(a => a.name == assetInfo.id);
            return asset;
        }
        private static void Finallize_BundleAssets(BundleReader.UnityBundle_Data bundle_Data)
        {
            foreach (var assets in bundle_Data.info.assets)
            {
                var assetbundle = assets.assetBundle;
                foreach (var assetInfo in assets.manifest)
                {
                    Debug.Log($"Checking Asset Info for -> {assetInfo.id}");
                    switch (assetInfo.AssetType)
                    {
                        case BundleAssetType.JSON:
                            Handle_JSON_Asset(bundle_Data, assetInfo);
                            break;
                    }
                }
            }
        }
        private static void Handle_JSON_Asset(BundleReader.UnityBundle_Data bundle_Data, BundleAssetInfo assetInfo)
        {
            try
            {
                bundle_Data.data.TryGetValue(assetInfo.path, out var data);
                var json = data as string;
                BundleRegistry.TryGetAsset_From_ID(bundle_Data.info.Name, assetInfo.id, out var obj);
                if (obj == null)
                {
                    Debug.Log("This asset is not registered.");
                    return;
                }
                if (string.IsNullOrWhiteSpace(json))
                {
                    Debug.Log("This asset has No JSON.");
                    return;
                }
                ReflectionUtils.Populate_From_JSON(obj, json, UnityBundler.JsonSettings);
                Register_Asset(assetInfo.type, assetInfo, obj);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Unable To Handle Json Asset : {assetInfo.name}");
                Debug.LogError(e);
            }
        }
    }
}
