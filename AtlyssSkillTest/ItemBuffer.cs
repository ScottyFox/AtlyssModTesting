using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
//WIP Perhaps i could buffer the Items used by players host-side//
namespace AtlyssSkillTest
{
    internal class ItemBuffer
    {
        public static Dictionary<PlayerInventory, List<ScriptableConsumable>> Consumed_Buffer = new Dictionary<PlayerInventory, List<ScriptableConsumable>>();
        public static void AddConsumed(PlayerInventory inventory,ScriptableConsumable consumable)
        {
            if (inventory == null)
                return;
            if (!Consumed_Buffer.TryGetValue(inventory, out var consumed_list))
            {
                consumed_list = new List<ScriptableConsumable>();
                Consumed_Buffer.Add(inventory,consumed_list);
            }
            consumed_list.Add(consumable);
        }
    }
}

namespace AtlyssSkillTest
{
    [HarmonyPatch]
    internal static class ItemPatch
    {
        [HarmonyPatch(typeof(PlayerInventory), "Init_UseConsumable")]
        [HarmonyPostfix]
        static void Init_UseConsumable_Postfix_Patch(ref PlayerInventory __instance, ScriptableConsumable _scriptConsumable, ItemData _itemData)
        {
            ItemBuffer.AddConsumed(__instance, _scriptConsumable);
        }
    }
    [HarmonyPatch]
    internal static class DebugTests
    {
        [HarmonyPatch(typeof(PlayerInventory), "Cmd_UseConsumable")]
        [HarmonyPostfix]
        static void Init_UseConsumable_Postfix_Patch(ref PlayerInventory __instance)
        {
            Debug.Log("Cmd_UseConsumable");
        }
    }
}
