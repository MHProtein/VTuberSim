using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEngine;
using VTuber.BattleSystem.Buff;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect;
using VTuber.BattleSystem.Effect.Conditions;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;

namespace VTuber.BattleSystem.Card
{

    public class VCard
    {
        public uint Id { get; private set; }
        public uint configID => _configuration.id;
        public string CardName => _configuration.cardName;
        public bool IsExhaust => _configuration.isExhaust;
        public bool IsUnique => _configuration.notRepeatable;
        public string CardType => _configuration.cardType;
   
        public string LiveType => _configuration.liveType;
        public List<string> Tags => _configuration.tags;
        public CostType CostType => _configuration.costType;
        public uint CostBuffId => _configuration.costBuffId;
        
        public VEffectCondition Condition => condition;
        private VEffectCondition condition;
        
        public int Cost => _cost.Value;
        private VUpgradableValue<int> _cost;
        public Sprite Icon => _configuration.icon;
        
        private List<VEffect> _effects;
        private List<VEffect> _newEffects;

        public bool IsUpgraded => _isUpgraded;
        public bool IsPrioritized => _configuration.prioritized;
        public bool IsTemporaryUpgraded => isTemporaryUpgraded;
        
        private bool _isUpgraded = false;
        private bool isTemporaryUpgraded = false;
        
        public List<VEffect> Effects
        {
            get
            {
                List<VEffect> effects = new List<VEffect>();
                effects.AddRange(_effects);
                if (_isUpgraded)
                {
                    effects.AddRange(_newEffects);
                }
                return effects;
            }
        }
        public VCardRarity Rarity => _configuration.rarity;
        
        public Action<bool> setPlayable;
        public Action<bool, int, int> popularityPreviewAction;
        public Action<bool, int, int> shieldPreviewAction;
        
        private readonly VCardConfiguration _configuration;
        
        public VCard(VCardConfiguration configuration, uint id, List<VEffectItem> effects, List<VEffectItem> newEffects, int conditionId)
        {
            _configuration = configuration;
            Id = id;
            _effects = new List<VEffect>();
            _effects = new List<VEffect>();
            _newEffects = new List<VEffect>();
            _cost = new VUpgradableValue<int>(configuration.cost, configuration.upgradedCost);
            if(conditionId != -1)
                condition = VDataManager.Instance.GetConditionByID((uint)conditionId);

            int i = 0;
            try
            {
                for (i = 0; i < effects.Count; i++)
                {
                    _effects.Add(effects[i].CreateEffect());
                }
            }
            catch (Exception e)
            {
                VDebug.LogError($"id为 {configID} 的卡牌 第{i}个效果配置错误");
                throw;
            }

            try
            {
                for (i = 0; i < _newEffects.Count; i++)
                {
                    _newEffects.Add(newEffects[i].CreateEffect());
                }
            }
            catch (Exception e)
            {
                VDebug.LogError($"id为 {configID} 的卡牌 第{i}个—“新”—效果配置错误");
                throw;
            }
        }
        
        public string GetDescription()
        {
            string des = _configuration.description; 
            if(des.Contains("X1"))
                des = des.Replace("X1", _effects[0].GetValue());
            if (des.Contains("X2"))
                des = des.Replace("X2", _effects[1].GetValue());
            if (des.Contains("X3"))
                des = des.Replace("X3", _effects[2].GetValue());
            if (des.Contains("X4"))
                des = des.Replace("X4", _effects[3].GetValue());
            
            if (_isUpgraded && !_configuration.upgradeDescription.IsNullOrWhitespace())
            {
                des += "\n" + _configuration.upgradeDescription;
                if (des.Contains("NX1"))
                    des = des.Replace("NX1", _newEffects[0].GetValue());
                if (des.Contains("NX2"))
                    des = des.Replace("NX2", _newEffects[1].GetValue());
            }
            return des;
        }

        public void Play()
        {
            VDebug.Log("卡牌打出: " + CardName);
            VDebug.Log("效果: " + _configuration.effects.Count);
            Dictionary<string, object> message = new Dictionary<string, object>()
            {
                { "Card", this },
                { "Effects", Effects },
                { "Cost", Cost },
                { "CostType", CostType },
                { "CostBuffId", CostBuffId }
            };
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnCardPlayed, message);
        }

        public void TestCondition(VBattle battle)
        {

            bool costSatisfied = false;
            if (CostType == CostType.Buff)
            {
                costSatisfied = battle.BuffManager.TestCost(CostBuffId, Cost);
            }
            else
            {
                costSatisfied = battle.BattleAttributeManager.StaminaManager.TestCost(Cost, CostType == CostType.TrueStamina);
            }
            
            bool conditionSatisfied = false;

            if (condition == null)
                conditionSatisfied = true;
            else
                conditionSatisfied = condition.IsTrue(battle, null);
            
            
            setPlayable?.Invoke(costSatisfied && conditionSatisfied);
        }

        public void PreviewPopularity(VBattle battle, bool firstTime)
        {
            int originalValue = 0;
            int finalValue = 0;
            foreach (var effect in _effects)
            {
                if(effect is IVValuePreview preview)
                {
                    if (preview.AttributeName == "BAParameter")
                    {
                        bool isTrue = true;
                        foreach (var c in effect.conditions)
                        {
                            if (!c.IsTrue(battle, null))
                                isTrue = false;
                        }
                        if(!isTrue)
                            continue;
                        int value = preview.GetValue(battle);
                        originalValue += value;
                        finalValue += battle.BattleAttributeManager.PreviewPopularityChange(value);
                    }
                }
            }
            popularityPreviewAction?.Invoke(firstTime, originalValue, finalValue);
        }

        public void PreviewShield(VBattle battle, bool firstTime)
        {
            int originalValue = 0;
            int finalValue = 0;
            foreach (var effect in _effects)
            {
                if (effect is IVValuePreview preview)
                {
                    if (preview.AttributeName == "BAShield")
                    {
                        bool isTrue = true;
                        foreach (var c in effect.conditions)
                        {
                            if (!c.IsTrue(battle, null))
                                isTrue = false;
                        }
                        if(!isTrue)
                            continue;
                        int value = preview.GetValue(battle);
                        originalValue += value;
                        finalValue += battle.BattleAttributeManager.PreviewShieldChange(value);
                    }
                    
                }
            }
            shieldPreviewAction?.Invoke(firstTime, originalValue, finalValue);
        }

        public void Upgrade(bool isTemporary)
        {
            if (_isUpgraded)
            {
                return;
            }
            
            _isUpgraded = true;
            isTemporaryUpgraded = isTemporary;
            _cost.Upgrade();
            
            foreach (var effect in _effects)
            {
                effect.Upgrade();
            }
            VDebug.Log("卡牌升级: " + CardName);
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnCardUpgraded, new Dictionary<string, object>()
            {
                { "Card", this }
            });
        }

        public void Downgrade()
        {
            if (!_isUpgraded)
            {
                return;
            }
            
            _isUpgraded = false;
            isTemporaryUpgraded = false;
            _cost.Downgrade();
            
            foreach (var effect in _effects)
            {
                effect.Downgrade();
            }
        }
    }
}