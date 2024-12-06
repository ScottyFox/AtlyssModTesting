﻿using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
namespace AtlyssSkillTest
{
    [HarmonyPatch]
    internal static class Patch
    {
        static FieldInfo pInventory_Field = AccessTools.Field(typeof(PlayerQuesting), "_pInventory");
        public static PlayerInventory GetPlayerInventory_PlayerQuesting(PlayerQuesting playerQuesting)
        {
            return (PlayerInventory)pInventory_Field.GetValue(playerQuesting);
        }
        [HarmonyPatch(typeof(PlayerQuesting), "<Client_CompleteQuest>g__Iterate_QuestItemRequirement|22_3")]
        [HarmonyPrefix]
        static void Client_CompleteQuest__Iterate_QuestItemRequirement_Prefix_Patch(ref PlayerQuesting __instance, ref QuestItemRequirement _qIR)
        {
            var player_item_count = Count_Item_In_PlayerInventory(GetPlayerInventory_PlayerQuesting(__instance), _qIR._questItem._itemName);
            var remaining_count = _qIR._itemsNeeded - player_item_count;
            Remove_Item_In_ItemStorage(_qIR._questItem._itemName, remaining_count);
        }
        [HarmonyPatch(typeof(PlayerQuesting), "Check_QuestCompletion")]
        [HarmonyPrefix]
        static void Check_QuestCompletion_Prefix_Patch(ref PlayerQuesting __instance, int _index)
        {
            ScriptableQuest current_quest = GameManager._current.LocateQuest(__instance._questProgressData[_index]._questTag);
            if (current_quest == null)
                return;
            var quest_requirements = current_quest._questObjective._questItemRequirements;
            var quest_progress = __instance._questProgressData[_index]._itemProgressValues;
            for (var i = 0; i < quest_requirements.Length; i++)
            {
                var item_name = quest_requirements[i]._questItem._itemName;
                var item_required = quest_requirements[i]._itemsNeeded;
                if (quest_progress[i] == item_required)
                    continue;
                var storage_item_count = Find_Item_In_ItemStorage(item_name);
                var actual_item_count = quest_progress[i] + storage_item_count;
                if (actual_item_count > item_required)
                    actual_item_count = item_required;
                __instance._questProgressData[_index]._itemProgressValues[i] = actual_item_count;
            }
        }
        static void ItemStorage_Load()
        {
            if (!ItemStorageManager._current._isOpen)
                ProfileDataManager._current.Load_ItemStorageData();
        }
        static void ItemStorage_Save()
        {
            if (ProfileDataManager._current._itemStorageProfile == null)
                ProfileDataManager._current.Load_ItemStorageData();
            ProfileDataManager._current.Save_ItemStorageData();
        }
        static int Find_Item_In_ItemStorage(string itemName)
        {
            int count = 0;
            ItemStorage_Load();
            count += Count_Item_In_ItemDatas(ItemStorageManager._current._itemDatas, itemName);
            count += Count_Item_In_ItemDatas(ItemStorageManager._current._itemDatas_01, itemName);
            count += Count_Item_In_ItemDatas(ItemStorageManager._current._itemDatas_02, itemName);
            return count;
        }
        static int Count_Item_In_ItemDatas(List<ItemData> itemList, string itemName)
        {
            int count = 0;
            foreach (ItemData item in itemList)
            {
                if (item._itemName == itemName)
                    count += item._quantity;
            }
            return count;
        }
        static int Count_Item_In_PlayerInventory(PlayerInventory inventory, string itemName)
        {
            return Count_Item_In_ItemDatas(inventory._heldItems, itemName);
        }
        static void Remove_Item_In_ItemStorage(string itemName, int remove_count)
        {
            ItemStorage_Load();
            var count = remove_count;
            count -= Remove_Item_In_ItemDatas(itemName, count, ref ItemStorageManager._current._itemDatas);
            count -= Remove_Item_In_ItemDatas(itemName, count, ref ItemStorageManager._current._itemDatas_01);
            count -= Remove_Item_In_ItemDatas(itemName, count, ref ItemStorageManager._current._itemDatas_02);
            ItemStorage_Save();
        }
        //Oh God My Eyes.
        static int Remove_Item_In_ItemDatas(string itemName, int count, ref List<ItemData> item_list)
        {
            if (count == 0) return 0;
            int remaining = count;
            for (int i = 0; i < item_list.Count && remaining > 0; i++)
            {
                var item = item_list[i];
                if (item._itemName != itemName)
                    continue;
                int tmp = item._quantity;
                item._quantity -= remaining;
                remaining -= tmp;
                if (item._quantity <= 0)
                {
                    item_list.RemoveAt(i);
                    i--;
                }
            }
            if (remaining < 0)
                remaining = 0;
            int removed_count = count - remaining;
            return removed_count;
        }
    }
}
