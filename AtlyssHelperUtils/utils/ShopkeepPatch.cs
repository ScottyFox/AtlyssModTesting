using HarmonyLib;
using System.Linq;
using UnityEngine;
namespace AtlyssHelperUtils
{
    [HarmonyPatch]
    internal class ShopkeepPatch
    {
        [HarmonyPatch(typeof(NetNPC), "Init_ShopkeepListing")]
        [HarmonyPrefix]
        static void Init_ShopkeepListing_Prefix_Patch(ref NetNPC __instance, ref ScriptableShopkeep _scriptShopkeep)
        {
            Debug.Log(_scriptShopkeep._shopName);
            if (!AtlyssUtils.Custom_Shopkeep_Items.TryGetValue(_scriptShopkeep._shopName, out var _items))
                return;
            foreach (var _item in _items)
            {
                Debug.Log(_item._itemName);
                ShopkeepItem newitem = new ShopkeepItem();
                newitem._itemNameTag = _item._itemName;
                newitem._scriptItem = _item;
                newitem._isInfiniteStock = true;
                Debug.Log(newitem._itemNameTag);
                foreach (var itemtable in _scriptShopkeep._shopkeepItemTables)
                {
                    itemtable._shopkeepItems = itemtable._shopkeepItems.Append(newitem).ToArray();
                }
            }
        }
    }
}
