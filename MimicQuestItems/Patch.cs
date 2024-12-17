using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
namespace SpikeQuestItems
{
    internal static class Utils
    {
        public static FieldInfo pInventory_Field = AccessTools.Field(typeof(PlayerQuesting), "_pInventory");

        public static PlayerInventory GetPlayerInventory_PlayerQuesting(PlayerQuesting playerQuesting)
        {
            return (PlayerInventory)pInventory_Field.GetValue(playerQuesting);
        }
        public static void ItemStorage_Load()
        {
            if (!ItemStorageManager._current._isOpen)
                ProfileDataManager._current.Load_ItemStorageData();
        }
        public static void ItemStorage_Save()
        {
            if (ProfileDataManager._current._itemStorageProfile == null)
                ProfileDataManager._current.Load_ItemStorageData();
            ProfileDataManager._current.Save_ItemStorageData();
        }
        public static int Find_Item_In_ItemStorage(string itemName)
        {
            int count = 0;
            ItemStorage_Load();
            count += Count_Item_In_ItemDatas(ItemStorageManager._current._itemDatas, itemName);
            count += Count_Item_In_ItemDatas(ItemStorageManager._current._itemDatas_01, itemName);
            count += Count_Item_In_ItemDatas(ItemStorageManager._current._itemDatas_02, itemName);
            return count;
        }
        public static int Count_Item_In_ItemDatas(List<ItemData> itemList, string itemName)
        {
            int count = 0;
            foreach (ItemData item in itemList)
            {
                if (item._itemName == itemName)
                    count += item._quantity;
            }
            return count;
        }
        public static int Count_Item_In_PlayerInventory(PlayerInventory inventory, string itemName)
        {
            return Count_Item_In_ItemDatas(inventory._heldItems, itemName);
        }
        public static int Count_Total_Items_In_Player_And_ItemStorage(PlayerInventory inventory, string itemName)
        {
            var player_item_count = Count_Item_In_PlayerInventory(inventory, itemName);
            var storage_item_count = Find_Item_In_ItemStorage(itemName);
            return player_item_count + storage_item_count;
        }
        public static void Remove_Item_In_ItemStorage(string itemName, int remove_count)
        {
            ItemStorage_Load();
            var count = remove_count;
            count -= Remove_Item_In_ItemDatas(itemName, count, ref ItemStorageManager._current._itemDatas);
            count -= Remove_Item_In_ItemDatas(itemName, count, ref ItemStorageManager._current._itemDatas_01);
            count -= Remove_Item_In_ItemDatas(itemName, count, ref ItemStorageManager._current._itemDatas_02);
            ItemStorage_Save();
        }
        //Oh God My Eyes.
        public static int Remove_Item_In_ItemDatas(string itemName, int count, ref List<ItemData> item_list)
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
    [HarmonyPatch]
    internal static class Dynamic_QuestItemRequirement_Patch
    {
        [HarmonyTargetMethod]
        public static MethodBase Find_Iterate_QuestItemRequirement()
        {
            var type = typeof(PlayerQuesting);
            var methods = AccessTools.GetDeclaredMethods(type);
            foreach (var method in methods)
            {
                if (method.Name.Contains("Iterate_QuestItemRequirement"))
                {
                    return method;
                }
            }
            throw new Exception("Unable to find 'Iterate_QuestItemRequirement'!!!");
        }
        [HarmonyPrefix]
        static void Client_CompleteQuest__Iterate_QuestItemRequirement_Prefix_Patch(ref PlayerQuesting __instance, ref QuestItemRequirement _qIR)
        {
            var player_item_count = Utils.Count_Item_In_PlayerInventory(Utils.GetPlayerInventory_PlayerQuesting(__instance), _qIR._questItem._itemName);
            var remaining_count = _qIR._itemsNeeded - player_item_count;
            Utils.Remove_Item_In_ItemStorage(_qIR._questItem._itemName, remaining_count);
        }
    }
    [HarmonyPatch]
    internal static class Patch
    {
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
                var player_item_count = Utils.Count_Item_In_PlayerInventory(Utils.GetPlayerInventory_PlayerQuesting(__instance), item_name);
                var storage_item_count = Utils.Find_Item_In_ItemStorage(item_name);
                var actual_item_count = player_item_count + storage_item_count;
                if (actual_item_count > item_required)
                    actual_item_count = item_required;
                __instance._questProgressData[_index]._itemProgressValues[i] = actual_item_count;
            }
        }
        public const string QuestProgressNote_ItemProgress_Pattern = @"^(?<Itemname>.+?): \((?<A>\d+) \/ (?<B>\d+)\)$";
        [HarmonyPatch(typeof(PlayerQuesting), "Apply_QuestProgressNote")]
        [HarmonyPrefix]
        static void Apply_QuestProgressNote_Prefix_Patch(ref PlayerQuesting __instance, ref string _string)
        {
            Match match = Regex.Match(_string, QuestProgressNote_ItemProgress_Pattern);
            if (!match.Success)
                return;
            string itemName = match.Groups["Itemname"].Value;
            int totalCount = Utils.Count_Total_Items_In_Player_And_ItemStorage(Utils.GetPlayerInventory_PlayerQuesting(__instance),itemName);
            int currentCount = int.Parse(match.Groups["A"].Value);
            int requiredCount = int.Parse(match.Groups["B"].Value);
            if (currentCount >= totalCount)
                return;
            currentCount = totalCount;
            if (currentCount > requiredCount)
                currentCount = requiredCount;
            _string = $"{itemName}: ({currentCount} / {requiredCount})*";
        }
    }
}
