using HarmonyLib;
using AtlyssHelperUtils;
namespace AtlyssSkillEqualizer
{
    [HarmonyPatch]
    internal static class Patch
    {
        [HarmonyPatch(typeof(GameManager), "Cache_ScriptableAssets")]
        [HarmonyPostfix]
        static void Cache_ScriptableAssets_Postfix_Patch(ref GameManager __instance)
        {
            var skills = AtlyssUtils.GetSkills(__instance);
            foreach (var skill in skills.Values)
            {
                foreach (var rank in skill._skillRanks)
                {
                    //Fetch Max
                    float highest_mod = rank._attackPowerModRef;
                    if (rank._magicPowerModRef > highest_mod)
                        highest_mod = rank._magicPowerModRef;
                    if (rank._rangePowerModRef > highest_mod)
                        highest_mod = rank._rangePowerModRef;
                    rank._attackPowerModRef = highest_mod;
                    rank._magicPowerModRef = highest_mod;
                    rank._rangePowerModRef = highest_mod;
                }
            }
        }
    }
}
