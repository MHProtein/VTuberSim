using System;
using System.Collections.Generic;
using CsvHelper;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.BattleSystem.Effect;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.StringToEnum;
using VTuber.Core.TypeSerialization;

namespace VTuber.BattleSystem.Buff
{
    public enum BuffType
    {
        Permanent,
        Persistent,
    }
    public class VBuffConfiguration
    {
        public int id;
        public string buffName;
        public Sprite icon;
        public BuffType buffType;
        public bool stackable = true;
        
        public List<int> effects;

        public VBuffConfiguration(CsvReader csv)
        {
            id = csv.GetField<int>("ID");
            buffName = csv.GetField<string>("Name");
            //icon = csv.GetField<string>("Icon");
            buffType = Enum.Parse<BuffType>(csv.GetField<string>("BuffType"));
            stackable = csv.GetField<int>("Stackable") == 1;

            effects = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                int? effect = csv.GetField<int?>("Effect" + (i + 1));
                if (effect.HasValue)
                {
                    effects.Add(effect.Value);
                }
            }
        }

        public VBuff CreateBuff()
        {
            return new VBuff(this, CreateEffects());
        }
        
        protected List<VEffect> CreateEffects()
        {
            List<VEffect> effectList = new List<VEffect>();
            foreach (var effectId in effects)
            {

                if (VBattleDataManager.Instance.EffectConfigurations.TryGetValue(effectId, out var config))
                {
                    effectList.Add(config.CreateEffect());
                }
                else
                {
                    VDebug.LogError($"Effect with ID {effectId} not found for buff {buffName}");
                }
            }
            return effectList;
        }
        
        public bool IsBuffPersistent()
        {
            return buffType == BuffType.Persistent;
        }
        
        public bool IsBuffPermanent()
        {
            return buffType == BuffType.Permanent;
        }
    }
}