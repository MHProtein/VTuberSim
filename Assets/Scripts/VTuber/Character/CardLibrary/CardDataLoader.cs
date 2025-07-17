using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Sirenix.Utilities;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Buff;
using VTuber.BattleSystem.Effect;
using VTuber.BattleSystem.Effect.Conditions;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;

namespace VTuber.Character
{
    public class VBattleResourcesLoader
    {
        private readonly string _xlsxPath;

        public VBattleResourcesLoader(string xlsxPath)
        {
            _xlsxPath = xlsxPath;
        }

        public List<VCardConfiguration> Load()
        {
            var workbook = new Workbook();
            workbook.LoadFromFile(_xlsxPath);

            LoadConditions(workbook);
            LoadEffects(workbook);
            LoadBuffs(workbook);
            return LoadCards(workbook);
        }

        private Worksheet Sheet(Workbook wb, string name)
        {
            var sheet = wb.Worksheets[name];
            if (sheet == null)
                throw new FileNotFoundException($"Worksheet '{name}' not found in {_xlsxPath}");
            return sheet;
        }

        private List<VCardConfiguration> LoadCards(Workbook wb)
        {
            var sheet = Sheet(wb, "Cards");
            var list = new List<VCardConfiguration>();
            
            for (int r = 1; r <= sheet.LastRow - 1; r++)
            {
                var row = sheet.Rows[r];
                if(row.Columns[VCardHeaderIndex.Id].Value.IsNullOrWhitespace())
                    continue; 
                var cfg = new VCardConfiguration(row);
                list.Add(cfg);
            }

            VBattleDataManager.Instance.SetCardConfigurations(list);
            return list;
        }

        private void LoadEffects(Workbook wb)
        {
            var sheet = Sheet(wb, "Effects");
            var list = new List<VEffectConfiguration>();

            for (int r = 1; r <= sheet.LastRow - 1; r++)
            {
                var row = sheet.Rows[r];
                var typeName = row.Columns[VEffectHeaderIndex.Type].Value;
                if(row.Columns[VEffectHeaderIndex.Id].Value.IsNullOrWhitespace())
                    continue; 
                var effectType = Type.GetType("VTuber.BattleSystem.Effect." + typeName + "Configuration");
                if (effectType == null)
                {
                    VDebug.LogError($"Effect type {typeName} not found.");
                    continue;
                }
                var effect = (VEffectConfiguration)Activator.CreateInstance(effectType, row);
                list.Add(effect);
            }

            VBattleDataManager.Instance.SetEffectConfigurations(list);
        }

        private void LoadBuffs(Workbook wb)
        {
            var sheet = Sheet(wb, "Buffs");
            var list = new List<VBuffConfiguration>();

            for (int r = 1; r <= sheet.LastRow - 1; r++)
            {
                var row = sheet.Rows[r];
                if(row.Columns[VBuffHeaderIndex.Id].Value.IsNullOrWhitespace())
                    continue; 
                var cfg = new VBuffConfiguration(row);
                list.Add(cfg);
            }

            VBattleDataManager.Instance.SetBuffConfigurations(list);
        }

        private void LoadConditions(Workbook wb)
        {
            var sheet = Sheet(wb, "Conditions");
            var list = new List<VEffectCondition>();

            for (int r = 1; r <= sheet.LastRow - 1; r++)
            {
                var row = sheet.Rows[r];
                if(row.Columns[VConditionHeaderIndex.Id].Value.IsNullOrWhitespace())
                    continue; 
                var typeName = row.Columns[VConditionHeaderIndex.Type].Value;
                var condType = Type.GetType("VTuber.BattleSystem.Effect.Conditions." + typeName);
                if (condType == null)
                {
                    VDebug.LogError($"Condition type {typeName} not found.");
                    continue;
                }
                var cond = (VEffectCondition)Activator.CreateInstance(condType, row);
                list.Add(cond);
            }

            VBattleDataManager.Instance.SetConditions(list);
        }
    }
}
