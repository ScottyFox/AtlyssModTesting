using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
namespace AtlyssHelperUtils
{
    public class AtlyssUtils
    {
        static FieldInfo cachedScriptableCombatElements_Field = AccessTools.Field(typeof(GameManager), "_cachedScriptableCombatElements");
        public static Dictionary<string, ScriptableCombatElement> GetCombatElements(GameManager manager)
        {
            return (Dictionary<string, ScriptableCombatElement>)cachedScriptableCombatElements_Field.GetValue(manager);
        }
        static FieldInfo cachedScriptableConditions_Field = AccessTools.Field(typeof(GameManager), "_cachedScriptableConditions");
        public static Dictionary<string, ScriptableCondition> GetConditions(GameManager manager)
        {
            return (Dictionary<string, ScriptableCondition>)cachedScriptableConditions_Field.GetValue(manager);
        }
        static FieldInfo cachedScriptableCreeps_Field = AccessTools.Field(typeof(GameManager), "_cachedScriptableCreeps");
        public static Dictionary<string, ScriptableCreep> GetCreeps(GameManager manager)
        {
            return (Dictionary<string, ScriptableCreep>)cachedScriptableCreeps_Field.GetValue(manager);
        }
        static FieldInfo cachedScriptableItems_Field = AccessTools.Field(typeof(GameManager), "_cachedScriptableItems");
        public static Dictionary<string, ScriptableItem> GetItems(GameManager manager)
        {
            return (Dictionary<string, ScriptableItem>)cachedScriptableItems_Field.GetValue(manager);
        }
        static FieldInfo cachedScriptablePlayerClasses_Field = AccessTools.Field(typeof(GameManager), "_cachedScriptablePlayerClasses");
        public static Dictionary<string, ScriptablePlayerBaseClass> GetPlayerClasses(GameManager manager)
        {
            return (Dictionary<string, ScriptablePlayerBaseClass>)cachedScriptablePlayerClasses_Field.GetValue(manager);
        }
        static FieldInfo cachedScriptableQuests_Field = AccessTools.Field(typeof(GameManager), "_cachedScriptableQuests");
        public static Dictionary<string, ScriptableQuest> GetQuests(GameManager manager)
        {
            return (Dictionary<string, ScriptableQuest>)cachedScriptableQuests_Field.GetValue(manager);
        }
        static FieldInfo cachedScriptableRaces_Field = AccessTools.Field(typeof(GameManager), "_cachedScriptableRaces");
        public static Dictionary<string, ScriptablePlayerRace> GetRaces(GameManager manager)
        {
            return (Dictionary<string, ScriptablePlayerRace>)cachedScriptableRaces_Field.GetValue(manager);
        }
        static FieldInfo cachedScriptableSkills_Field = AccessTools.Field(typeof(GameManager), "_cachedScriptableSkills");
        public static Dictionary<string, ScriptableSkill> GetSkills(GameManager manager)
        {
            return (Dictionary<string, ScriptableSkill>)cachedScriptableSkills_Field.GetValue(manager);
        }
        static FieldInfo cachedScriptableStatModifiers_Field = AccessTools.Field(typeof(GameManager), "_cachedScriptableStatModifiers");
        public static Dictionary<string, ScriptableStatModifier> GetStatModifiers(GameManager manager)
        {
            return (Dictionary<string, ScriptableStatModifier>)cachedScriptableStatModifiers_Field.GetValue(manager);
        }
        public static void Add_Item(ScriptableItem item)
        {
            if (GameManager._current == null)
                return;
            AtlyssUtils.GetItems(GameManager._current).Add(item._itemName, item);
        }
        public static void Add_Skill(ScriptableSkill skill, bool create_scroll = false)
        {
            if (GameManager._current == null)
                return;
            GetSkills(GameManager._current).Add(skill._skillName, skill);
            if (create_scroll)
            {
                var scroll_item = ItemUtils.Convert_Skill_To_SkillScroll(skill);
                Add_Item(scroll_item);
            }
        }
    }
}