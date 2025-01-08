using System.Collections.Generic;
using UnityEngine;
namespace ScottyFoxArt.UnityBundler
{
    public static class BundleRegistry
    {
        private static Dictionary<string, System.Action<UnityEngine.Object>> Type_Registerers = new();
        private static Dictionary<string, System.Func<string, UnityEngine.Object>> Reference_Fetchers = new()
        {{"unitybundler_id_ref",ID_Reference_Fetcher}};
        private static Dictionary<string, Dictionary<string, UnityEngine.Object>> Registered_IDs = new();
        private static Dictionary<string, Dictionary<string, string>> Registered_Paths = new();
        private static HashSet<UnityEngine.Object> Cached_Objects = new();
        public static void Clear_Cache()
        { Cached_Objects.Clear(); }
        //Type
        public static void Register_TypeRegisterer_Dict(Dictionary<string, System.Action<UnityEngine.Object>> dict)
        {
            foreach (var pair in dict)
            {
                Type_Registerers[pair.Key] = pair.Value;
            }
        }
        public static void Register_TypeRegisterer(string typeName, System.Action<UnityEngine.Object> action)
        {
            Type_Registerers[typeName] = action;
        }
        public static void Remove_TypeRegisterer(string typeName)
        {
            if (Type_Registerers.ContainsKey(typeName))
                Type_Registerers.Remove(typeName);
        }
        public static bool TryGet_TypeRegisterer(string typeName, out System.Action<UnityEngine.Object> action)
        {
            return Type_Registerers.TryGetValue(typeName, out action);
        }
        //References
        public static void Register_Reference_Fetcher_Dict(Dictionary<string, System.Func<string, UnityEngine.Object>> dict)
        {
            foreach (var pair in dict)
            {
                Reference_Fetchers[pair.Key] = pair.Value;
            }
        }
        public static void Register_Reference_Fetcher(string name, System.Func<string, UnityEngine.Object> func)
        {
            Reference_Fetchers[name] = func;
        }
        public static bool TryGet_Reference(string reference, out UnityEngine.Object obj, bool cache = false, string prefferedType = null)
        {
            obj = null;
            foreach (var fetcher in Reference_Fetchers.Values)
            {
                obj = fetcher?.Invoke(reference);
                if (obj != null)
                {
                    if (cache)
                        Cached_Objects.Add(obj);
                    return true;
                }
            }
            //Finally Use Unity's Resources.
            obj = Unity_Resources_Reference_Fetcher(reference);
            if (obj != null)
            {
                if (cache)
                    Cached_Objects.Add(obj);
                return true;
            }
            return false;
        }
        //Bundle IDs
        private static Dictionary<string, UnityEngine.Object> FetchBundle(string bundleName, bool createNew = false)
        {
            if (!Registered_IDs.TryGetValue(bundleName, out var bundle) && createNew)
            {
                bundle = new Dictionary<string, UnityEngine.Object>();
                Registered_IDs[bundleName] = bundle;
            }
            return bundle;
        }
        //
        public static void Register(string bundleName, string ID, UnityEngine.Object obj)
        {
            bundleName = bundleName.ToLower();
            ID = ID.ToLower();
            var bundle = FetchBundle(bundleName, true);
            if (bundle.ContainsKey(ID))
                Debug.Log($"Bundle {bundleName} already contains ID {ID}, overwriting");
            bundle[ID] = obj;
        }
        public static void RegisterPath(string bundleName, string path, string ID)
        {
            path = string.Join("/", path.Split(['/', '\\']));
            if (!string.IsNullOrWhiteSpace(bundleName))
            {
                //Clean Path
                if (Check_BundlePrefix(path, out var cleanPath, out string cleanBundleName))
                {
                    path = cleanPath;
                    bundleName = cleanBundleName;
                }
            }
            bundleName = bundleName.ToLower();
            if (!Registered_Paths.TryGetValue(bundleName, out var bundlePaths))
            {
                bundlePaths = new();
                Registered_Paths.Add(bundleName, bundlePaths);
            }
            bundlePaths[path.ToLower()] = ID.ToLower();
        }
        public static void RegisterPath(string path, string ID)
        {
            RegisterPath(string.Empty, path, ID);
        }
        public static bool TryGetID_From_Path(string bundleName, string path, out string ID)
        {
            ID = string.Empty;
            bundleName = bundleName.ToLower();
            path = string.Join("/", path.ToLower().Split(['/', '\\']));
            //path = path.Substring(path.LastIndexOf('/') + 1);
            if (Registered_Paths.TryGetValue(bundleName, out var bundlePaths))
                if (bundlePaths.TryGetValue(path, out ID))
                    return true;
            return false;
        }
        public static bool TryGetID_From_Path(string path, out string ID, out string bundleName)
        {
            ID = string.Empty;
            path = string.Join("/", path.ToLower().Split(['/', '\\']));
            if (!Check_BundlePrefix(path, out string cleanPath, out bundleName))
            {
                cleanPath = path;
                bundleName = "";
            }
            if (Registered_Paths.TryGetValue(bundleName, out var bundlePaths))
                if (bundlePaths.TryGetValue(cleanPath, out ID))
                    return true;
            return false;
        }
        public static bool TryGetAsset_From_ID(string bundleName, string ID, out UnityEngine.Object obj)
        {
            obj = null;
            bundleName = bundleName.ToLower();
            if (TryGetID_From_Path(bundleName, ID, out var pID))
            {
                Debug.Log($"Found ID From path -> {ID} -> {pID}");
                ID = pID;
            }
            else
            {
                ID = ID.ToLower();
                Debug.Log($"{bundleName} -> {ID}");
            }
            var bundle = FetchBundle(bundleName, false);
            if (bundle == null)
                return false;
            if (bundle.TryGetValue(ID, out obj))
                return true;
            //TODO adjust this vvv to fix specific path references.?
            foreach (var key in bundle.Keys)
            {
                if (key.Contains(ID))
                {
                    obj = bundle[key];
                    return true;
                }
            }
            return false;
        }
        public static bool TryGetAsset_From_ID(string ID, out UnityEngine.Object obj)
        {
            obj = null;
            if (TryGetID_From_Path(ID, out var p_ID, out var bundleName))
            {
                Debug.Log($"ID found in path -> {p_ID} -> {bundleName}");
                ID = p_ID;
            }
            return TryGetAsset_From_ID(bundleName, ID, out obj);
        }
        public static bool Check_BundlePrefix(string path, out string cleanPath, out string bundleName)
        {
            Debug.Log(path);
            cleanPath = string.Empty;
            bundleName = string.Empty;
            if (path.IndexOf(UnityBundler.BUNDLE_ASSET_PREFIX_TAG) != 0)
                return false;
            var splitter_index = path.IndexOfAny(['/', '\\']);
            if (splitter_index == -1)
                return false;
            bundleName = path.Substring(1, splitter_index - 1);
            cleanPath = path.Substring(splitter_index + 1);
            return true;
        }
        public static bool TryGetBundleAsset_From_ID(string ID, out Object reference)
        {
            reference = null;
            if (!Check_BundlePrefix(ID, out var path, out var bundleName))
                return false;
            Debug.Log($"Bundle Name : {bundleName} -> {ID} -> {path}");
            if (!TryGetAsset_From_ID(bundleName, path, out var result))
                return false;
            reference = result;
            return true;
        }
        private static UnityEngine.Object ID_Reference_Fetcher(string id)
        {
            TryGetBundleAsset_From_ID(id, out var obj);
            return obj;
        }
        private static UnityEngine.Object Unity_Resources_Reference_Fetcher(string id)
        {
            var lowerId = id.ToLower();
            var splitter = lowerId.LastIndexOf('/');
            var path = string.Empty;
            var name = lowerId;
            if (splitter != -1)
            {
                path = lowerId.Substring(0, splitter + 1);
                name = lowerId.Substring(path.Length);
            }
            if (path == "")
            {
                Debug.LogWarning("Empty Resource Paths May Result in laggy loading, try a more specific path.");
            }
            var resources = Resources.LoadAll(path, typeof(object));
            foreach (var resource in resources)
            {
                if (resource.name.ToLower().StartsWith(name))
                {
                    if (resource)
                        return resource;
                }
            }
            return null;
        }
    }
}
