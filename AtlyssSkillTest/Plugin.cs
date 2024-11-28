using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
namespace AtlyssSkillTest
{
    public static class PluginInfo
    {
        public const string GUID = "AtlyssSkillTest";
        public const string NAME = "AtlyssSkillTest";
        public const string VERSION = "0.0.0";
        public const string WEBSITE = "";
    }
    [BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static Harmony instance = new(PluginInfo.GUID);
        private void Awake()
        {
            try
            {
                RegisterAssetBundles();
                Assets.PopulateAssets();
                instance.PatchAll(Assembly.GetExecutingAssembly());
                Logger.LogInfo($"Plugin {PluginInfo.GUID} is loaded!");
            }
            catch (Exception exception)
            {
                Logger.LogInfo($"Plugin {PluginInfo.GUID} failed to load...");
                Logger.LogError(exception);
            }
        }
        public static void RegisterAssetBundles()
        {

        }
    }
    public static class Assets
    {
        // Replace mbundle with the Asset Bundle Name from your unity project 
        public static string mainAssetBundleName = "testskills";
        public static AssetBundle MainAssetBundle = null;

        private static string GetAssemblyName() => Assembly.GetExecutingAssembly().FullName.Split(',')[0];
        public static void PopulateAssets()
        {
            Debug.LogWarning("Loading Main Assets");
            Debug.LogWarning(GetAssemblyName());
            if (MainAssetBundle == null)
            {
                using (var aStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(GetAssemblyName() + "." + mainAssetBundleName))
                {
                    MainAssetBundle = AssetBundle.LoadFromStream(aStream);
                }
            }
        }
    }
}
