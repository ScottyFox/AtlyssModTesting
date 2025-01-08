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
            var shareitems_scroll = ItemUtils.Convert_Skill_To_SkillScroll(shareitems_skill);
            AtlyssUtils.Add_Skill(shareitems_skill);
            AtlyssUtils.Add_Item(shareitems_scroll);
            ShopkeepUtils.Register_Shopkeep_Item("Sally's Store", shareitems_scroll);
        }
    }
}
