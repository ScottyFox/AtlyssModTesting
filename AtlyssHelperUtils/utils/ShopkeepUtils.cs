//WIP//
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UnityEngine;
namespace AtlyssHelperUtils
{
    public static class ShopkeepUtils
    {
        public static Dictionary<string, List<CustomShopkeepItem>> Custom_Shopkeep_Listings = new Dictionary<string, List<CustomShopkeepItem>>();
        public static HashSet<string> Ignored_Vanilla_Shopkeep_Listings = new HashSet<string>();
        public static void Reset_Custom_Shopkeep_Listings()
        {
            Custom_Shopkeep_Listings.Clear();
            Ignored_Vanilla_Shopkeep_Listings.Clear();
        }
        public static void Ignore_Shopkeep_Items(string shopkeep_name)
        {
            Ignored_Vanilla_Shopkeep_Listings.Add(shopkeep_name);
        }
        public static void Register_Shopkeep_Item(string shopkeep_name, ScriptableItem item, bool infinite_stock = true, bool is_level_exclusive = false, int level = 0, bool relativeLevels = true)
        {
            if (!Custom_Shopkeep_Listings.TryGetValue(shopkeep_name, out var item_list))
            {
                item_list = new List<CustomShopkeepItem>();
                Custom_Shopkeep_Listings.Add(shopkeep_name, item_list);
            }
            ShopkeepItem newitem = new ShopkeepItem();
            newitem._itemNameTag = item._itemName;
            newitem._scriptItem = item;
            newitem._isInfiniteStock = infinite_stock;
            item_list.Add(new CustomShopkeepItem(newitem, is_level_exclusive, level));
        }
        public static void Handle_Shopkeep_Inventory(ScriptableShopkeep shopkeep)
        {
            if (Ignored_Vanilla_Shopkeep_Listings.Contains(shopkeep._shopName))
            {
                shopkeep._shopkeepItemTables = default;
            }
            Handle_Vanilla_Shopkeep_Inventory(shopkeep);
        }
        private static void Handle_Vanilla_Shopkeep_Inventory(ScriptableShopkeep shopkeep)
        {
            if (!Custom_Shopkeep_Listings.TryGetValue(shopkeep._shopName, out var _items))
                return;
            foreach (var _item in _items)
            {
                if (!_item.LevelExclusive)
                    Add_Item_To_Every_ItemTable(_item, shopkeep);
                else
                {
                    if (Add_Item_To_Nearest_ItemTable(_item, shopkeep))
                        continue;
                    if (_item.UseRelativeLevelListings)
                        if (Add_Item_To_Relative_ItemTable(_item, shopkeep))
                            continue;
                    Add_Item_To_New_ItemTable(_item, shopkeep);
                }
            }
        }
        private static void Add_Item_To_Every_ItemTable(CustomShopkeepItem item, ScriptableShopkeep shopkeep)
        {
            foreach (var itemtable in shopkeep._shopkeepItemTables)
            {
                itemtable._shopkeepItems = itemtable._shopkeepItems.Append(item.Item).ToArray();
            }
        }
        private static bool Add_Item_To_Nearest_ItemTable(CustomShopkeepItem item, ScriptableShopkeep shopkeep)
        {
            var nearest_itemtable = Find_Level_ItemTable(item.Level, shopkeep._shopkeepItemTables);
            if (nearest_itemtable != -1)
            {
                Add_Item_To_ItemTable(item.Item, shopkeep._shopkeepItemTables[nearest_itemtable]);
                return true;
            }
            return false;
        }
        private static bool Add_Item_To_Relative_ItemTable(CustomShopkeepItem item, ScriptableShopkeep shopkeep)
        {
            var nearest_itemtable = Find_Nearest_Level_ItemTable(item.Level, shopkeep._shopkeepItemTables);
            if (nearest_itemtable != -1)
            {
                var new_table = Copy_ItemTable(shopkeep._shopkeepItemTables[nearest_itemtable]);
                Add_Item_To_ItemTable(item.Item, new_table);
                shopkeep._shopkeepItemTables = shopkeep._shopkeepItemTables.Append(new_table).ToArray();
                return true;
            }
            return false;
        }
        private static void Add_Item_To_New_ItemTable(CustomShopkeepItem item, ScriptableShopkeep shopkeep)
        {
            var new_table = new ShopkeepItemTable
            {
                _shopkeepTableTag = "",
                _shopkeepItems = [item.Item],
                _levelRequirement = item.Level
            };
            shopkeep._shopkeepItemTables = shopkeep._shopkeepItemTables.Append(new_table).ToArray();
        }
        private static void Add_Item_To_ItemTable(ShopkeepItem item, ShopkeepItemTable itemTable)
        {
            itemTable._shopkeepItems = itemTable._shopkeepItems.Append(item).ToArray();
        }
        private static int Find_Level_ItemTable(int level, ShopkeepItemTable[] itemTables)
        {
            for (int i = 0; i < itemTables.Length; i++)
            {
                if (itemTables[i]._levelRequirement == level)
                    return i;
            }
            return -1;
        }
        private static int Find_Nearest_Level_ItemTable(int level, ShopkeepItemTable[] itemTables)
        {
            int nearest_index = -1;
            for (int i = 0; i < itemTables.Length; i++)
            {
                if (itemTables[i]._levelRequirement <= level)
                    if (nearest_index == -1)
                        nearest_index = i;
                    else if (itemTables[i]._levelRequirement > itemTables[nearest_index]._levelRequirement)
                        nearest_index = i;
            }
            return nearest_index;
        }
        private static ShopkeepItemTable Copy_ItemTable(ShopkeepItemTable itemTable)
        {
            return new ShopkeepItemTable
            {
                _shopkeepTableTag = itemTable._shopkeepTableTag,
                _shopkeepItems = itemTable._shopkeepItems,
                _levelRequirement = itemTable._levelRequirement
            };
        }
    }
    public struct CustomShopkeepItem
    {
        public ShopkeepItem Item;
        public bool LevelExclusive;
        public int Level;
        public bool UseRelativeLevelListings;
        public CustomShopkeepItem(ShopkeepItem item, bool is_level_exclusive = false, int level = 0, bool relativeLevels = true)
        {
            Item = item;
            LevelExclusive = is_level_exclusive;
            Level = level;
            UseRelativeLevelListings = relativeLevels;
        }
    }
}
