using Newtonsoft.Json;
using System.Collections.Generic;
namespace ScottyFoxArt.UnityBundler.Json
{
    [System.Serializable]
    public class BundleInfo
    {
        public string Name;
        public string Description;
        public string Version;
        public string Author;
        public List<string> meta;
        public List<BundleAssets> assets;
    }
    [System.Serializable]
    public class BundleAssets
    {
        public string assetBundle;
        public List<BundleAssetInfo> manifest;
    }
    [System.Serializable]
    public class BundleAssetInfo
    {
        public string name;
        public string type;
        public string id;
        public string path;
        [JsonIgnore]
        public BundleAssetType AssetType = BundleAssetType.Asset;
    }
    public enum BundleAssetType
    {
        Asset,
        JSON
    }
}