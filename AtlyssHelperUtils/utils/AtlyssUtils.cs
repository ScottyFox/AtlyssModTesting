using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
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
        public static ItemData Convert_To_ItemData(ScriptableItem scriptableItem, int slot = 0, string modifier = "")
        {
            ItemData itemData = new ItemData
            {
                _itemName = scriptableItem._itemName,
                _isAltWeapon = false,
                _isEquipped = false,
                _maxQuantity = scriptableItem._maxStackAmount,
                _quantity = 0,
                _slotNumber = slot,
                _modifierTag = modifier
            };
            return itemData;
        }
        //Items
        public static void Add_Item(ScriptableItem item)
        {
            if (GameManager._current == null)
                return;
            GetItems(GameManager._current).Add(item._itemName, item);
        }
        //Skills
        public static void Add_Skill(ScriptableSkill skill, bool create_scroll = false)
        {
            if (GameManager._current == null)
                return;
            GetSkills(GameManager._current).Add(skill._skillName, skill);
            if (create_scroll)
            {
                var scroll_item = Convert_Skill_To_Scroll_Item(skill);
                Add_Item(scroll_item);
            }
        }
        public static void Convert_To_Bonus_Skill(ScriptableSkill skill)
        {
            skill._allowAsBonusSkill = true;
        }
        public static ScriptableSkillScroll Convert_Skill_To_Scroll_Item(ScriptableSkill skill, int vendor_cost = 1000, ScriptablePlayerBaseClass baseClass = null)
        {
            ScriptableSkillScroll skillScroll = ScriptableObject.CreateInstance<ScriptableSkillScroll>();
            skillScroll._scriptableSkill = skill;
            skillScroll._baseClassRequirement = baseClass;
            skillScroll._levelRequirement = skill._skillRankParams._baseLevelRequirement;
            skillScroll._consumableObject = Resources.Load<GameObject>("_consumableEffect_skillScroll");
            skillScroll._consumableCooldown = 1.5f;
            skillScroll._vendorCost = vendor_cost;
            skillScroll._itemType = ItemType.CONSUMABLE;
            skillScroll._itemName = $"""Skill Scroll({skill._skillName})""";
            skillScroll._itemDescription = $"""Teaches the user {skill._skillName}.""";
            switch (skill._skillDamageType)
            {
                case DamageType.Mind:
                    skillScroll._itemIcon = Resources.Load<Sprite>("_graphic/_ui/_itemicons/_consumable/_conIco_15");
                    break;
                case DamageType.Dexterity:
                    skillScroll._itemIcon = Resources.Load<Sprite>("_graphic/_ui/_itemicons/_consumable/_conIco_10");
                    break;
                case DamageType.Strength:
                    skillScroll._itemIcon = Resources.Load<Sprite>("_graphic/_ui/_itemicons/_consumable/_conIco_13");
                    break;
                default:
                    skillScroll._itemIcon = Resources.Load<Sprite>("_graphic/_ui/_itemicons/_consumable/_conIco_18");
                    break;
            }
            return skillScroll;
        }
        //Shop Keep Helper//
        public static Dictionary<string, HashSet<ScriptableItem>> Custom_Shopkeep_Items = new Dictionary<string, HashSet<ScriptableItem>>();
        public static void Clear_Custom_Shopkeeper_Items()
        {
            Custom_Shopkeep_Items.Clear();
        }
        public static void Register_Item_To_Shopkeeper(string shopkeep_name, ScriptableItem item)
        {
            if (!Custom_Shopkeep_Items.TryGetValue(shopkeep_name, out var item_list))
            {
                item_list = new HashSet<ScriptableItem>();
                Custom_Shopkeep_Items[shopkeep_name] = item_list;
            }
            item_list.Add(item);
        }
    }
}