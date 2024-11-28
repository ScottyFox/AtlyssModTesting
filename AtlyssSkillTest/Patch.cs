using HarmonyLib;
using AtlyssHelperUtils;
namespace AtlyssSkillTest
{
    [HarmonyPatch]
    internal static class Patch
    {
        [HarmonyPatch(typeof(GameManager), "Cache_ScriptableAssets")]
        [HarmonyPostfix]
        static void Cache_ScriptableAssets_Postfix_Patch(ref GameManager __instance)
        {
            var shareitems_skill = Assets.MainAssetBundle.LoadAsset<ScriptableSkill>("skill_shareitem");
            var shareitems_scroll = AtlyssUtils.Convert_Skill_To_Scroll_Item(shareitems_skill);
            AtlyssUtils.Add_Skill(shareitems_skill);
            AtlyssUtils.Add_Item(shareitems_scroll);
            AtlyssUtils.Register_Item_To_Shopkeeper("Sally's Store", shareitems_scroll);
        }
    }
}
