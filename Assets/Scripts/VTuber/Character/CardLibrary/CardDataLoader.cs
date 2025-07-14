using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using VTuber.BattleSystem.Card;
using CsvHelper;
using VTuber.BattleSystem.Buff;
using VTuber.BattleSystem.Effect;
using VTuber.BattleSystem.Effect.Conditions;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;

namespace VTuber.Character
{
    public class BattleResourcesLoader
    {
        private string _cardDataPath;
        private string _effectDataPath;
        private string _buffDataPath;
        private string _conditionDataPath;
        
        public BattleResourcesLoader(string path)
        {
            _cardDataPath = Path.Join(path, "Cards.csv");
            _effectDataPath = Path.Join(path, "Effects.csv");
            _buffDataPath = Path.Join(path, "Buffs.csv");
            _conditionDataPath = Path.Join(path, "Conditions.csv");
        }

        public List<VCardConfiguration> Load()
        {
            LoadConditions();
            LoadEffects();
            LoadBuffs();
            return LoadCards();
        }
        
        private List<VCardConfiguration> LoadCards()
        {
            List<VCardConfiguration> cardConfigurations = new List<VCardConfiguration>();
            
            using (var reader = new StreamReader(_cardDataPath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    VCardConfiguration card = new VCardConfiguration(csv);
                    cardConfigurations.Add(card);
                }
            }
            
            VBattleDataManager.Instance.SetCardConfigurations(cardConfigurations);
            return cardConfigurations;
        }

        private void LoadEffects()
        {
            List<VEffectConfiguration> effectConfigurations = new List<VEffectConfiguration>();
            
            using (var reader = new StreamReader(_effectDataPath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    Type effectType = Type.GetType("VTuber.BattleSystem.Effect." + csv.GetField<string>("Type") + "Configuration");
                    
                    if (effectType is null)
                    {
                        VDebug.LogError($"Effect type {csv.GetField<string>("Type")} not found.");
                        continue;
                    }
                    
                    VEffectConfiguration effect = (VEffectConfiguration)Activator.CreateInstance(effectType, csv);
                    effectConfigurations.Add(effect);
                }
            }
            
            VBattleDataManager.Instance.SetEffectConfigurations(effectConfigurations);
        }

        private void LoadBuffs()
        {
            List<VBuffConfiguration> buffConfigurations = new List<VBuffConfiguration>();

            using (var reader = new StreamReader(_buffDataPath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    VBuffConfiguration buff = new VBuffConfiguration(csv);
                    buffConfigurations.Add(buff);
                }
            }

            VBattleDataManager.Instance.SetBuffConfigurations(buffConfigurations);
        }
        
        
        private void LoadConditions()
        {
            List<VEffectCondition> conditions = new List<VEffectCondition>();

            using (var reader = new StreamReader(_conditionDataPath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    Type conditionType = Type.GetType("VTuber.BattleSystem.Effect.Conditions." + csv.GetField<string>("Type"));
                    
                    if (conditionType is null)
                    {
                        VDebug.LogError($"Effect type {csv.GetField<string>("Type")} not found.");
                        continue;
                    }
                    
                    VEffectCondition condition = (VEffectCondition)Activator.CreateInstance(conditionType, csv);
                    conditions.Add(condition);
                }
            }

            VBattleDataManager.Instance.SetConditions(conditions);
        }
    }
}