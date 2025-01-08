using HarmonyLib;
using AtlyssHelperUtils;
namespace ClassSkillUniversal
{
    [HarmonyPatch]
    internal static class Patch
    {
        [HarmonyPatch(typeof(GameManager), "Cache_ScriptableAssets")]
        [HarmonyPostfix]
        static void Cache_ScriptableAssets_Postfix_Patch(ref GameManager __instance)
        {
            var Skills = AtlyssUtils.GetSkills(__instance);
            var Classes = AtlyssUtils.GetPlayerClasses(__instance);
            foreach (var player_class in Classes.Values)
            {
                foreach (var skill in player_class._classSkills)
                {
                    skill._allowAsBonusSkill = true;
                    var skill_scroll = ItemUtils.Convert_Skill_To_SkillScroll(skill);
                    AtlyssUtils.Add_Item(skill_scroll);
                    ShopkeepUtils.Register_Shopkeep_Item("Sally's Store", skill_scroll);
                }
            }
        }
    }
}
