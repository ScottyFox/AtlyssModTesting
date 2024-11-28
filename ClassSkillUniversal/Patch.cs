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
            AtlyssUtils.Clear_Custom_Shopkeeper_Items();
            var Skills = AtlyssUtils.GetSkills(__instance);
            var Classes = AtlyssUtils.GetPlayerClasses(__instance);
            foreach (var player_class in Classes.Values)
            {
                foreach (var skill in player_class._classSkills)
                {
                    AtlyssUtils.Convert_To_Bonus_Skill(skill);
                    var skill_scroll = AtlyssUtils.Convert_Skill_To_Scroll_Item(skill);
                    AtlyssUtils.Add_Item(skill_scroll);
                    AtlyssUtils.Register_Item_To_Shopkeeper("Sally's Store", skill_scroll);
                }
            }
        }
    }
}
