﻿using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Reflection;
namespace AtlyssHelperUtils
{
    public static class PluginInfo
    {
        public const string GUID = "AtlyssHelperUtils";
        public const string NAME = "AtlyssHelperUtils";
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