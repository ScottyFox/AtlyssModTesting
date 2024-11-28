using HarmonyLib;
using AtlyssHelperUtils;
using ClosedXML.Excel;
using System;
using System.IO;
namespace AtlyssObserver
{
    [HarmonyPatch]
    internal static class Patch
    {
        [HarmonyPatch(typeof(GameManager), "Cache_ScriptableAssets")]
        [HarmonyPostfix]
        static void Cache_ScriptableAssets_Postfix_Patch(ref GameManager __instance)
        {
            var creeps = AtlyssUtils.GetCreeps(__instance);
            using (var workbook = new XLWorkbook())
            {
                var creep_worksheet = workbook.Worksheets.Add("Creep Data");
                int r = 1;
                int c = 1;
                foreach (var creep in creeps.Values)
                {
                    var calculated_stats = StatUtils.Creep_Calculate_StatStruct(creep);
                    var calculated_stats_2 = StatUtils.Creep_Calculate_StatStruct(creep,2);
                    var calculated_stats_3 = StatUtils.Creep_Calculate_StatStruct(creep,3);
                    var calculated_stats_4 = StatUtils.Creep_Calculate_StatStruct(creep,4);
                    creep_worksheet.Cell(r, c).Value = "Name"; c++;
                    creep_worksheet.Cell(r, c).Value = creep._creepName; c++;
                    r++; c = 1;
                    creep_worksheet.Cell(r, c).Value = "Level"; c++;
                    creep_worksheet.Cell(r, c).Value = creep._creepLevel.ToString(); c++;
                    r++; c = 1;
                    creep_worksheet.Cell(r, c).Value = "Damage Type"; c++;
                    creep_worksheet.Cell(r, c).Value = Enum.GetName(typeof(DamageType), creep._creepDamageType); c++;
                    r++; c = 1;
                    creep_worksheet.Cell(r, c).Value = "Base Damage"; c++;
                    creep_worksheet.Cell(r, c).Value = creep._baseDamage.ToString(); c++;
                    r++; c = 1;
                    creep_worksheet.Cell(r, c).Value = "==Stat Table==";
                    r++; c = 1;
                    creep_worksheet.Cell(r, c).Value = "Health"; c++;
                    creep_worksheet.Cell(r, c).Value = calculated_stats._maxHealth.ToString(); c++;
                    creep_worksheet.Cell(r, c).Value = calculated_stats_2._maxHealth.ToString(); c++;
                    creep_worksheet.Cell(r, c).Value = calculated_stats_3._maxHealth.ToString(); c++;
                    creep_worksheet.Cell(r, c).Value = calculated_stats_4._maxHealth.ToString(); c++;
                    r++; c = 1;
                    creep_worksheet.Cell(r, c).Value = "Mana"; c++;
                    creep_worksheet.Cell(r, c).Value = calculated_stats._maxMana.ToString(); c++;
                    r++; c = 1;
                    creep_worksheet.Cell(r, c).Value = "Stamina"; c++;
                    creep_worksheet.Cell(r, c).Value = calculated_stats._maxStamina.ToString(); c++;
                    r++; c = 1;
                    creep_worksheet.Cell(r, c).Value = "Experience"; c++;
                    creep_worksheet.Cell(r, c).Value = StatUtils.Creep_Experience(creep,1, calculated_stats).ToString(); c++;
                    creep_worksheet.Cell(r, c).Value = StatUtils.Creep_Experience(creep, 2, calculated_stats_2).ToString(); c++;
                    creep_worksheet.Cell(r, c).Value = StatUtils.Creep_Experience(creep, 3, calculated_stats_3).ToString(); c++;
                    creep_worksheet.Cell(r, c).Value = StatUtils.Creep_Experience(creep, 4, calculated_stats_4).ToString(); c++;
                    r++;
                    r++; c = 1;
                    creep_worksheet.Cell(r, c).Value = "Attack Power"; c++;
                    creep_worksheet.Cell(r, c).Value = calculated_stats._attackPower.ToString(); c++;
                    r++; c = 1;
                    creep_worksheet.Cell(r, c).Value = "Dex Power"; c++;
                    creep_worksheet.Cell(r, c).Value = calculated_stats._dexPower.ToString(); c++;
                    r++; c = 1;
                    creep_worksheet.Cell(r, c).Value = "Magic Power"; c++;
                    creep_worksheet.Cell(r, c).Value = calculated_stats._magicPower.ToString(); c++;
                    r++;
                    r++; c = 1;
                    creep_worksheet.Cell(r, c).Value = "Critical Rate"; c++;
                    creep_worksheet.Cell(r, c).Value = (calculated_stats._criticalRate * 100).ToString() + "%"; c++;
                    r++; c = 1;
                    creep_worksheet.Cell(r, c).Value = "Magic Critical Rate"; c++;
                    creep_worksheet.Cell(r, c).Value = (calculated_stats._magicCriticalRate * 100).ToString() + "%"; c++;
                    r++;
                    r++; c = 1;
                    creep_worksheet.Cell(r, c).Value = "Defense"; c++;
                    creep_worksheet.Cell(r, c).Value = calculated_stats._defense.ToString(); c++;
                    r++; c = 1;
                    creep_worksheet.Cell(r, c).Value = "Magic Defense"; c++;
                    creep_worksheet.Cell(r, c).Value = calculated_stats._defense.ToString(); c++;
                    r++; c = 1;
                    creep_worksheet.Cell(r, c).Value = "Evasion"; c++;
                    creep_worksheet.Cell(r, c).Value = (calculated_stats._evasion * 100).ToString() + "%"; c++;
                    r++; c = 1;
                    creep_worksheet.Cell(r, c).Value = "==Loot Table==";
                    r++; c = 1;
                    foreach(var drop in creep._itemDrops)
                    {
                        creep_worksheet.Cell(r, c).Value = drop._item._itemName; c++;
                        creep_worksheet.Cell(r, c).Value = "Chance:"; c++;
                        creep_worksheet.Cell(r, c).Value = (drop._dropChance*100).ToString()+"%"; c++;
                        creep_worksheet.Cell(r, c).Value = "Quantity:"; c++;
                        creep_worksheet.Cell(r, c).Value = drop._dropQuantity.ToString(); c++;
                        r++; c = 1;
                    }
                    r++;
                }
                workbook.SaveAs(Path.Combine(BepInEx.Paths.PluginPath,"CreepInfo.xlsx"));
            }
        }
    }
}
