using HarmonyLib;
using System;
namespace AtlyssHelperUtils.Events
{
    [HarmonyPatch]
    public static class AtlyssEvents
    {
        public static event Action OnGameAssetsCached;
        public static bool GameAssetsCached { get; private set; } = false;
        [HarmonyPatch(typeof(GameManager), "Cache_ScriptableAssets")]
        [HarmonyPostfix]
        static void Cache_ScriptableAssets_Postfix_Patch(ref GameManager __instance)
        {
            GameAssetsCached = true;
            AtlyssEvents.OnGameAssetsCached?.Invoke();
        }
        //WIP//
        public static event Action OnCustomCombatElementsLoaded;
        public static event Action OnCustomConditionsLoaded;
        public static event Action OnCustomCreepsLoaded;
        public static event Action OnCustomItemsLoaded;
        public static event Action OnCustomPlayerClassesLoaded;
        public static event Action OnCustomQuestsLoaded;
        public static event Action OnCustomRacesLoaded;
        public static event Action OnCustomSkillsLoaded;
        public static event Action OnCustomStatModifiersLoaded;
    }
}
