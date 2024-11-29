using HarmonyLib;
using System.Linq;
using UnityEngine;
namespace AtlyssHelperUtils.Patches
{
    [HarmonyPatch]
    internal class ShopkeepPatch
    {
        [HarmonyPatch(typeof(NetNPC), "Init_ShopkeepListing")]
        [HarmonyPrefix]
        static void Init_ShopkeepListing_Prefix_Patch(ref NetNPC __instance, ref ScriptableShopkeep _scriptShopkeep)
        {
            ShopkeepUtils.Handle_Shopkeep_Inventory(_scriptShopkeep);
        }
    }
}
