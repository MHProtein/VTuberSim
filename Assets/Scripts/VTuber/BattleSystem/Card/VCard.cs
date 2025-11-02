using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect;
using VTuber.BattleSystem.Effect.Conditions;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;

namespace VTuber.BattleSystem.Card
{
    public class VCardSaveData
    {
        public uint configID;
        public uint id;
        public bool isUpgraded;
    }

    public class VCard
    {
        private readonly VCardConfiguration _configuration;
        private readonly VUpgradableValue<int> _cost;

        private readonly List<VEffect> _effects;
        private readonly List<VEffect> _newEffects;

        public Action<bool, int, int> popularityPreviewAction;

        public Action<bool> setPlayable;
        public Action<bool, int, int> shieldPreviewAction;

        public VCard(VCardConfiguration configuration, uint id, List<VEffectItem> effects, List<VEffectItem> newEffects,
            int conditionId)
        {
            _configuration = configuration;
            Id = id;
            _effects = new List<VEffect>();
            _effects = new List<VEffect>();
            _newEffects = new List<VEffect>();
            _cost = new VUpgradableValue<int>(configuration.cost, configuration.upgradedCost);
            if (conditionId != -1)
                Condition = VDataManager.Instance.GetConditionByID((uint)conditionId);

            var i = 0;
            try
            {
                for (i = 0; i < effects.Count; i++) _effects.Add(effects[i].CreateEffect());
            }
            catch (Exception e)
            {
                VDebug.LogError($"id为 {configID} 的卡牌 第{i}个效果配置错误");
                throw;
            }

            try
            {
                for (i = 0; i < _newEffects.Count; i++) _newEffects.Add(newEffects[i].CreateEffect());
            }
            catch (Exception e)
            {
                VDebug.LogError($"id为 {configID} 的卡牌 第{i}个—“新”—效果配置错误");
                throw;
            }
        }

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

        public VEffectCondition Condition { get; }

        public int Cost => _cost.Value;
        public Sprite Icon => _configuration.icon;

        public bool IsUpgraded { get; private set; }

        public bool IsPrioritized => _configuration.prioritized;
        public bool IsTemporaryUpgraded { get; private set; }

        public List<VEffect> Effects
        {
            get
            {
                var effects = new List<VEffect>();
                effects.AddRange(_effects);
                if (IsUpgraded) effects.AddRange(_newEffects);
                return effects;
            }
        }

        public VCardRarity Rarity => _configuration.rarity;

        public string GetDescription()
        {
            var des = _configuration.description;
            if (des.Contains("X1"))
                des = des.Replace("X1", _effects[0].GetValue());
            if (des.Contains("X2"))
                des = des.Replace("X2", _effects[1].GetValue());
            if (des.Contains("X3"))
                des = des.Replace("X3", _effects[2].GetValue());
            if (des.Contains("X4"))
                des = des.Replace("X4", _effects[3].GetValue());

            if (IsUpgraded && !_configuration.upgradeDescription.IsNullOrWhitespace())
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
            var message = new Dictionary<string, object>
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
            var costSatisfied = false;
            if (CostType == CostType.Buff)
                costSatisfied = battle.BuffManager.TestCost(CostBuffId, Cost);
            else
                costSatisfied =
                    battle.BattleAttributeManager.StaminaManager.TestCost(Cost, CostType == CostType.TrueStamina);

            var conditionSatisfied = false;

            if (Condition == null)
                conditionSatisfied = true;
            else
                conditionSatisfied = Condition.IsTrue(battle, null);


            setPlayable?.Invoke(costSatisfied && conditionSatisfied);
        }

        public void PreviewPopularity(VBattle battle, bool firstTime)
        {
            var originalValue = 0;
            var finalValue = 0;
            foreach (var effect in _effects)
                if (effect is IVValuePreview preview)
                    if (preview.AttributeName == "BAParameter")
                    {
                        var isTrue = true;
                        foreach (var c in effect.conditions)
                            if (!c.IsTrue(battle, null))
                                isTrue = false;
                        if (!isTrue)
                            continue;
                        var value = preview.GetValue(battle);
                        originalValue += value;
                        finalValue += battle.BattleAttributeManager.PreviewPopularityChange(value);
                    }

            popularityPreviewAction?.Invoke(firstTime, originalValue, finalValue);
        }

        public void PreviewShield(VBattle battle, bool firstTime)
        {
            var originalValue = 0;
            var finalValue = 0;
            foreach (var effect in _effects)
                if (effect is IVValuePreview preview)
                    if (preview.AttributeName == "BAShield")
                    {
                        var isTrue = true;
                        foreach (var c in effect.conditions)
                            if (!c.IsTrue(battle, null))
                                isTrue = false;
                        if (!isTrue)
                            continue;
                        var value = preview.GetValue(battle);
                        originalValue += value;
                        finalValue += battle.BattleAttributeManager.PreviewShieldChange(value);
                    }

            shieldPreviewAction?.Invoke(firstTime, originalValue, finalValue);
        }

        public void Upgrade(bool isTemporary, bool notify = true)
        {
            if (IsUpgraded) return;

            IsUpgraded = true;
            IsTemporaryUpgraded = isTemporary;
            _cost.Upgrade();

            foreach (var effect in _effects) effect.Upgrade();

            if (!notify)
                return;
            VDebug.Log("卡牌升级: " + CardName);
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnCardUpgraded, new Dictionary<string, object>
            {
                { "Card", this }
            });
        }

        public void Downgrade()
        {
            if (!IsUpgraded) return;

            IsUpgraded = false;
            IsTemporaryUpgraded = false;
            _cost.Downgrade();

            foreach (var effect in _effects) effect.Downgrade();
        }

        public VCardSaveData Save()
        {
            return new VCardSaveData
            {
                configID = configID,
                id = Id,
                isUpgraded = IsUpgraded
            };
        }

        public void Load(VCardSaveData saveData)
        {
            Id = saveData.id;
            if (saveData.isUpgraded)
                Upgrade(false);
        }
    }
}