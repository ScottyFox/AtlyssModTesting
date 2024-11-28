using System;
namespace AtlyssHelperUtils.Customization.Json
{
    [Serializable]
    public class Item_Consumable_JSON : Item_JSON
    {
        public new string ItemType = "Consumable";
        public string LevelRequirement = null;
        public string ClassRequirement = null;
        public bool UseOnlyInTown = false;
        public string ConsumableObjectName = null;
    }
}
