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
using VTuber.Consumable;
using VTuber.CoopSystem;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.RaisingEffect;
using VTuber.EventSystem.Events;
using VTuber.Relic;
using VTuber.ScheduleSystem.Events;
using VTuber.ScheduleSystem.Events.DialogueEvent;
using VCardHeaderIndex = VTuber.BattleSystem.Card.VCardHeaderIndex;

namespace VTuber.Character
{
    public class VResourcesLoader
    {
        private readonly string _cardPath;
        private readonly string _raisingPath;
        private readonly string _relicsPath;
        private readonly string _coopPath;

        public VResourcesLoader(string cardPath, string raisingPath, string relicsPath, string coopPath)
        {
            _cardPath = cardPath;
            _raisingPath = raisingPath;
            _relicsPath = relicsPath;
            _coopPath = coopPath;
        }

        public List<VCardConfiguration> Load()
        {
            var cardWb = new Workbook();
            var raisingWb = new Workbook();
            var relicsWb = new Workbook();
            var coopWb = new Workbook();
            cardWb.LoadFromFile(_cardPath);
            raisingWb.LoadFromFile(_raisingPath);
            relicsWb.LoadFromFile(_relicsPath);
            coopWb.LoadFromFile(_coopPath);

            LoadConditions(cardWb);
            LoadEffects(cardWb);
            LoadBuffs(cardWb);
            LoadCardConditions(raisingWb);
            LoadPhaseEndingCondition(raisingWb);
            LoadPlacingConditions(raisingWb);
            LoadRaisingEffects(raisingWb);
            LoadDialogueEvents(raisingWb);
            LoadStreamEvents(raisingWb);
            LoadRelicConditions(relicsWb);
            LoadRelics(relicsWb);
            LoadCoopEvents(coopWb);
            LoadConsumables(relicsWb);
            return LoadCards(cardWb);
        }

        private Worksheet Sheet(Workbook wb, string name)
        {
            var sheet = wb.Worksheets[name];
            if (sheet == null)
                throw new FileNotFoundException($"Worksheet '{name}'");
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

            VResourcesManager.Instance.SetCardConfigurations(list);
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
                var effectType = Type.GetType("VTuber.BattleSystem.Effect." + typeName.Trim() + "Configuration");
                if (effectType == null)
                {
                    VDebug.LogError($"Effect type {typeName} not found.");
                    continue;
                }
                var effect = (VEffectConfiguration)Activator.CreateInstance(effectType, row);
                list.Add(effect);
            }

            VResourcesManager.Instance.SetEffectConfigurations(list);
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

            VResourcesManager.Instance.SetBuffConfigurations(list);
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
                var condType = Type.GetType("VTuber.BattleSystem.Effect.Conditions." + typeName.Trim());
                if (condType == null)
                {
                    VDebug.LogError($"Condition type {typeName} not found.");
                    continue;
                }
                var cond = (VEffectCondition)Activator.CreateInstance(condType, row);
                list.Add(cond);
            }

            VResourcesManager.Instance.SetConditions(list);
        }

        public void LoadRaisingEffects(Workbook wb)
        {
            var sheet = Sheet(wb, "RaisingEffects");
            var list = new List<VRaisingEffectConfiguration>();

            for (int r = 1; r <= sheet.LastRow - 1; r++)
            {
                var row = sheet.Rows[r];
                var typeName = row.Columns[VRaisingEffectHeaderIndex.Type].Value;
                if(row.Columns[VRaisingEffectHeaderIndex.Id].Value.IsNullOrWhitespace())
                    continue; 
                var effectType = Type.GetType("VTuber.Core.RaisingEffect." + typeName.Trim() + "Configuration");
                if (effectType == null)
                {
                    VDebug.LogError($"Raising Effect type {typeName} not found.");
                    continue;
                }
                var effect = (VRaisingEffectConfiguration)Activator.CreateInstance(effectType, row);
                list.Add(effect);
            }

            VResourcesManager.Instance.SetRaisingEffectConfigurations(list);
        }
        
        public void LoadCardConditions(Workbook wb)
        {
            var sheet = Sheet(wb, "CardConditions");
            var list = new List<VCardCondition>();

            for (int r = 1; r <= sheet.LastRow - 1; r++)
            {
                var row = sheet.Rows[r];
                var typeName = row.Columns[VCardConditionHeaderIndex.Type].Value;
                if(row.Columns[VCardConditionHeaderIndex.Id].Value.IsNullOrWhitespace())
                    continue; 
                var conditionType = Type.GetType("VTuber.Core.RaisingEffect." + typeName.Trim());
                if (conditionType == null)
                {
                    VDebug.LogError($"Card Condition type {typeName} not found.");
                    continue;
                }
                var condition = (VCardCondition)Activator.CreateInstance(conditionType, row);
                list.Add(condition);
            }

            VResourcesManager.Instance.SetCardConditions(list);
        }

        public void LoadPlacingConditions(Workbook wb)
        {
            var sheet = Sheet(wb, "PlacingConditions");
            var list = new List<VPlacingCondition>();
            
            for (int r = 1; r <= sheet.LastRow - 1; r++)
            {
                var row = sheet.Rows[r];
                var typeName = row.Columns[VPlacingConditionHeaderIndex.Type].Value;
                if(row.Columns[VPlacingConditionHeaderIndex.Id].Value.IsNullOrWhitespace())
                    continue; 
                var conditionType = Type.GetType("VTuber.EventSystem.Events." + typeName.Trim());
                if (conditionType == null)
                {
                    VDebug.LogError($"Placing Condition type {typeName} not found.");
                    continue;
                }
                var condition = (VPlacingCondition)Activator.CreateInstance(conditionType, row);
                list.Add(condition);
            }

            VResourcesManager.Instance.SetPlacingConditon(list);
        }

        public void LoadPhaseEndingCondition(Workbook wb)
        {
            var sheet = Sheet(wb, "PhaseEndingConditions");
            var list = new List<VPhaseEndingCondition>();

            for (int r = 1; r <= sheet.LastRow - 1; r++)
            {
                var row = sheet.Rows[r];
                var typeName = row.Columns[VPhaseEndingConditionHeaderIndex.Type].Value;
                if(row.Columns[VPhaseEndingConditionHeaderIndex.Id].Value.IsNullOrWhitespace())
                    continue; 
                var conditionType = Type.GetType("VTuber.ScheduleSystem.Events." + typeName.Trim());
                if (conditionType == null)
                {
                    VDebug.LogError($"Card Condition type {typeName} not found.");
                    continue;
                }
                var condition = (VPhaseEndingCondition)Activator.CreateInstance(conditionType, row);
                list.Add(condition);
            }

            VResourcesManager.Instance.SetPhaseEndingConditions(list);
        }

        public void LoadDialogueEvents(Workbook wb)
        {
            var sheet = Sheet(wb, "DialogueEvents");
            var list = new List<VDialogueEventConfiguration>();

            for (int r = 1; r <= sheet.LastRow - 1; r++)
            {
                var row = sheet.Rows[r];
                if(row.Columns[VCardConditionHeaderIndex.Id].Value.IsNullOrWhitespace())
                    continue;
                var condition = new VDialogueEventConfiguration(row);
                list.Add(condition);
            }

            VResourcesManager.Instance.SetDialogueEventConfigurations(list);
        }
        
        public void LoadStreamEvents(Workbook wb)
        {
            var sheet = Sheet(wb, "StreamEvents");
            var list = new List<VStreamEventConfiguration>();

            for (int r = 1; r <= sheet.LastRow - 1; r++)
            {
                var row = sheet.Rows[r];
                if(row.Columns[VCardConditionHeaderIndex.Id].Value.IsNullOrWhitespace())
                    continue;
                var condition = new VStreamEventConfiguration(row);
                list.Add(condition);
            }

            VResourcesManager.Instance.SetStreamEventConfigurations(list);
        }
        
        public void LoadRelicConditions(Workbook wb)
        {
            var sheet = Sheet(wb, "RaisingRelicConditions");
            
            var list = new List<VRaisingRelicCondition>();

            for (int r = 1; r <= sheet.LastRow - 1; r++)
            {
                var row = sheet.Rows[r];
                var typeName = row.Columns[VRaisingRelicConditionHeaderIndex.Type].Value;
                if(row.Columns[VRaisingRelicConditionHeaderIndex.Id].Value.IsNullOrWhitespace())
                    continue; 
                var conditionType = Type.GetType("VTuber.Relic." + typeName.Trim());
                if (conditionType == null)
                {
                    VDebug.LogError($"Card Condition type {typeName} not found.");
                    continue;
                }
                var condition = (VRaisingRelicCondition)Activator.CreateInstance(conditionType, row);
                list.Add(condition);
            }

            VResourcesManager.Instance.SetRelicConditions(list);
        }
        
        public void LoadRelics(Workbook wb)
        {
            var sheet = Sheet(wb, "Relics");
            var list = new List<VRelicConfiguration>();

            for (int r = 1; r <= sheet.LastRow - 1; r++)
            {
                var row = sheet.Rows[r];
                var typeName = row.Columns[VRelicHeaderIndex.Type].Value;
                if(row.Columns[VRelicHeaderIndex.Id].Value.IsNullOrWhitespace())
                    continue; 
                var type = Type.GetType("VTuber.Relic." + typeName.Trim() + "Configuration");
                if (type == null)
                {
                    VDebug.LogError($"Card Condition type {typeName} not found.");
                    continue;
                }
                var relic = (VRelicConfiguration)Activator.CreateInstance(type, row);
                list.Add(relic);
            }

            VResourcesManager.Instance.SetRelics(list);
        }

        public void LoadCoopEvents(Workbook wb)
        {
            var sheet = Sheet(wb, "CoopEvents");
            var list = new List<VCoopEvent>();

            for (int r = 1; r <= sheet.LastRow - 1; r++)
            {
                var row = sheet.Rows[r];
                var typeName = row.Columns[VRelicHeaderIndex.Type].Value;
                if(row.Columns[VCoopEventHeaderIndex.Id].Value.IsNullOrWhitespace())
                    continue; 
                var coopEvent = new VCoopEvent(row);
                list.Add(coopEvent);
            }

            VResourcesManager.Instance.SetCoopEvents(list);
        }
        
        public void LoadConsumables(Workbook wb)
        {
            var sheet = Sheet(wb, "Consumables");
            var list = new List<VConsumableConfiguration>();

            for (int r = 1; r <= sheet.LastRow - 1; r++)
            {
                var row = sheet.Rows[r];
                var typeName = row.Columns[VConsumableHeaderIndex.Type].Value;
                if(row.Columns[VConsumableHeaderIndex.Id].Value.IsNullOrWhitespace())
                    continue; 
                var consumableType = Type.GetType("VTuber.Consumable." + typeName.Trim() + "Configuration");
                if (consumableType == null)
                {
                    VDebug.LogError($"Consumable Condition type {typeName} not found.");
                    continue;
                }
                var consumable = (VConsumableConfiguration)Activator.CreateInstance(consumableType, row);
                list.Add(consumable);
            }

            VResourcesManager.Instance.SetConsumableConfigurations(list);
        }
        
    }
}
