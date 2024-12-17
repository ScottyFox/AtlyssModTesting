using BepInEx;
using HarmonyLib;
using System;
using System.Reflection;
namespace SpikeQuestItems
{
    public static class PluginInfo
    {
        public const string GUID = "scottyfoxart.atlyss.spikequestitems";
        public const string NAME = "Spike Quest Items";
        public const string VERSION = "1.0.2";
        public const string WEBSITE = "https://github.com/ScottyFox/AtlyssModTesting/tree/main/MimicQuestItems";
    }
    [BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static Harmony instance = new(PluginInfo.GUID);
        private void Awake()
        {
            try
            {
                instance.PatchAll(Assembly.GetExecutingAssembly());
                Logger.LogInfo($"Plugin {PluginInfo.GUID} is loaded!");
            }
            catch (Exception exception)
            {
                Logger.LogInfo($"Plugin {PluginInfo.GUID} failed to load...");
                Logger.LogError(exception);
            }
        }
    }
}
