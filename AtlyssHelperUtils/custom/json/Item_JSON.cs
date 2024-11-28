using System;
namespace AtlyssHelperUtils.Customization.Json
{
    [Serializable]
    public class Item_JSON
    {
        public string Name = null;
        public string Description = null;
        public bool UseGameIcon = false;
        public string IconPath = null;
        public string ItemType = null;
        public string Rarity = null;
        public int MaxStackAmount = 1;
        public int VendorCost = 10;
        public string Quest = null;
        public bool DestroyOnDrop = false;
        //ShopKeepers
        //IsGambleItem
        //ShopLevelRequirment
        //public List<string> ListTest = new List<string>();
    }
}
