using HarmonyLib;
using ScottyFoxArt.UnityBundler;
namespace AtlyssBundler.Patches
{
    [HarmonyBefore(["AtlyssHelperUtils"])]
    [HarmonyPatch]
    public static class Patches
    {
        [HarmonyPatch(typeof(GameManager), "Cache_ScriptableAssets")]
        [HarmonyPostfix]
        static void Cache_ScriptableAssets_Postfix_Patch(ref GameManager __instance)
        {
            Registry.Auto_Register_Types();
            Registry.Register_With_UnityBundler();
            UnityBundler.Register_UnityBundles_At_Directory(BepInEx.Paths.PluginPath);
            UnityBundler.Finallize_UnityBundles();
        }
    }
}
