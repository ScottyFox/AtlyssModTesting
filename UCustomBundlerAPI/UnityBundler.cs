using ScottyFoxArt.UnityBundler.Files;
using ScottyFoxArt.UnityBundler.Json;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
//Runtime Package For UnityBundler
namespace ScottyFoxArt.UnityBundler
{
    public class UnityBundler
    {
        public const string BUNDLE_DATA_VERSION = "unitybundle0";
        public const string BUNDLE_DATA_COMPRESSED_BUNDLE_TAG = "_unityBundle.zip";
        public const string BUNDLE_DATA_ASSETBUNDLE_BUNDLE_TAG = "_unityBundle.unity3d";
        public const string BUNDLE_DATA_FILENAME = "bundle.json";
        //Checks//
        public const string BUNDLE_ASSET_PREFIX_TAG = "$";
        public const string JSON_TYPE_TAG = ".JSON|";
        private static bool IsFinallize = false;

        public static JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            Converters = [
                new UnityBundle_Reference_JsonConverter(),
                new StringEnumConverter(),
                new UnityColorJsonConverter()
            ]
        };
        public static void Register_UnityBundles_At_Directory(string path)
        {
            Verify_If_IsFinallized();
            var found_bundles = BundleReader.Fetch_UnityBundles_At_Directory(path);
            foreach (var bundle in found_bundles)
            {
                BundleBuilder.Register(bundle);
            }
        }
        public static void Finallize_UnityBundles()
        {
            Verify_If_IsFinallized();
            IsFinallize = true;
            BundleBuilder.Finallize();
        }
        public static bool Verify_If_IsFinallized(bool useWarning = true)
        {
            if (IsFinallize && useWarning)
            {
                Debug.LogWarning("!!!UNITY BUNDLES HAVE ALREADY BEEN FINALLIZED!!!");
                Debug.LogWarning("IT IS RECOMMENDED THAT YOU DO NOT REGISTER OR FINALLIZE UNITYBUNDLES");
                Debug.LogWarning("AFTER THINGS HAVE ALREADY BEEN FINALLIZED");
                Debug.LogWarning("THIS CAN CAUSE UNINTENTIONAL BEHAVIOUR");
            }
            return IsFinallize;
        }
    }
}
