//WIP//
using UnityEngine;

namespace AtlyssHelperUtils
{
    public static class ItemUtils
    {
        public static ScriptableItem Generate_ScriptableItem(ItemType itemType)
        {
            ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
            item._itemType = itemType;
            return item;
        }
        public static ItemData Convert_To_ItemData(ScriptableItem scriptableItem, string modifier = "")
        {
            ItemData itemData = new ItemData
            {
                _itemName = scriptableItem._itemName,
                _isAltWeapon = false,
                _isEquipped = false,
                _maxQuantity = scriptableItem._maxStackAmount,
                _quantity = 0,
                _slotNumber = 0,
                _modifierTag = modifier
            };
            return itemData;
        }
        public static ScriptableSkillScroll Convert_Skill_To_SkillScroll(ScriptableSkill skill, int vendor_cost = 1000, ScriptablePlayerBaseClass baseClass = null, Sprite itemicon = null)
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
            if (itemicon == null)
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
            else
                skillScroll._itemIcon = itemicon;
            return skillScroll;
        }
    }
}
