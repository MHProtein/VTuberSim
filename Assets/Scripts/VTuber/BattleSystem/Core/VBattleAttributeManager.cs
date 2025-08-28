using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.Core.UI;

namespace VTuber.BattleSystem.Core
{

    public class VStaminaManager
    {
        public VValueModifier<float> ConsumeRateModifier => consumeRateModifier;
        protected VValueModifier<float> consumeRateModifier;
        
        public VValueModifier<int> ConsumePointsModifier => consumePointsModifier;
        protected VValueModifier<int> consumePointsModifier;

        VBattleStaminaAttribute _staminaAttribute;
        VBattleStaminaAttribute _shieldAttribute;
        public VStaminaManager(VBattleStaminaAttribute staminaAttribute, VBattleStaminaAttribute shieldAttribute)
        {
            _staminaAttribute = staminaAttribute;
            _shieldAttribute = shieldAttribute;
            consumePointsModifier = new VValueModifier<int>(0);
            consumeRateModifier = new VValueModifier<float>(0.0f);
        }
        
        public void ApplyCost(int cost, bool ignoreShield = false)
        {
            int calculatedCost = CalculateCost(cost);
            
            int costAfterShield = calculatedCost;
            if (!ignoreShield)
            {
                costAfterShield = calculatedCost - _shieldAttribute.Value;

                _shieldAttribute.AddTo(-calculatedCost >= 0 ? 0 : -calculatedCost, false);
                if (costAfterShield <= 0)
                    return;
            }
            
            _staminaAttribute.AddTo(-costAfterShield >= 0 ? 0 : -costAfterShield, false);
        }

        public bool TestCost(int cost, bool ignoreShield = false)
        {
            int calculatedCost = CalculateCost(cost);

            int costAfterShield = calculatedCost;
            if (!ignoreShield)
            {
                costAfterShield = calculatedCost - _shieldAttribute.Value;
            
                if (costAfterShield <= 0)
                    return true;
            }
            
            return _staminaAttribute.TestCost(-costAfterShield >= 0 ? 0 : -costAfterShield);
        }
        
        public int CalculateCost(int delta)
        {
            delta = (int)(delta * (1.0f - VValueModifier<int>.GetModifierFloatValue(consumeRateModifier)))
                    - VValueModifier<int>.GetModifierIntValue(consumePointsModifier);

            return delta;
        }

        public void Reset()
        {
            consumeRateModifier.Reset();
            consumePointsModifier.Reset();
        }

        public void OnTurnEnd()
        {
            foreach (var mod in consumePointsModifier.Modifiers)
            {
                if (mod.Value.DecreaseTurnCount())
                {
                    consumePointsModifier.RemoveModifier(mod.Key);
                }
            }
            foreach (var mod in consumeRateModifier.Modifiers)
            {
                if (mod.Value.DecreaseTurnCount())
                {
                    consumeRateModifier.RemoveModifier(mod.Key);
                }
            }
        }
        
    }

    public class VMultiplierManager
    {
        public VBattleMultiplierAttribute Multiplier { get; private set; }
        public List<VBattleMultiplierAttribute> Multipliers => _multiplierAttributes;
        private List<VBattleMultiplierAttribute> _multiplierAttributes;
        private List<int> multiplierSequence;
        private int _currentTurnIndex = 0;
        public VMultiplierManager(int mainAttributeIndex, 
            int maxConsecutiveMultiplierCount, List<int> abilityTurnCounts, 
            VBattleMultiplierAttribute singingMultiplierAttribute, 
            VBattleMultiplierAttribute gamingMultiplierAttribute,
            VBattleMultiplierAttribute chattingMultiplierAttribute,
            VBattleTurnAttribute turnAttribute)
        {
            _multiplierAttributes = new List<VBattleMultiplierAttribute>
            {
                singingMultiplierAttribute, //red
                gamingMultiplierAttribute, //yellow
                chattingMultiplierAttribute //blue
            };
            
            multiplierSequence = new List<int>(new int[turnAttribute.MaxTurn]);
            multiplierSequence[0] = mainAttributeIndex;
            multiplierSequence[^1] = mainAttributeIndex;

            GenerateMultiplierSequence(turnAttribute.MaxTurn, maxConsecutiveMultiplierCount, mainAttributeIndex, abilityTurnCounts);
        }
        
        public void OnEnable()
        {
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnTurnBegin, OnTurnBegin);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnTurnChange, OnTurnChange);
        }

        public void OnDisable()
        {
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnTurnBegin, OnTurnBegin);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnTurnChange, OnTurnChange);
        }

        private void OnTurnChange(Dictionary<string, object> messagedict)
        {
            int delta = (int)messagedict["Delta"];
            if (delta <= 0)
                return;

            if (multiplierSequence is null)
                return;
            for (int i = 0; i < delta; i++)
            {
                multiplierSequence.Add(multiplierSequence.Last());
            }
        }
        
        private void OnTurnBegin(Dictionary<string, object> messagedict)
        {
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnRotateMultiplier, new Dictionary<string, object>()
            {
                { "Name", _multiplierAttributes[multiplierSequence[_currentTurnIndex]].AttributeName },
                { "NewValue", _multiplierAttributes[multiplierSequence[_currentTurnIndex]].Value },
                { "Color", _multiplierAttributes[multiplierSequence[_currentTurnIndex]].color },
            });
            Multiplier = _multiplierAttributes[multiplierSequence[_currentTurnIndex]];
            if (_currentTurnIndex <= multiplierSequence.Count - 1)
            {
                _currentTurnIndex++;
                VDebug.Log("_currentTurnIndex: " + _currentTurnIndex);
            }
            VDebug.Log(Multiplier.AttributeName + " Value : " + Multiplier.Value);
        }

        private void GenerateMultiplierSequence(
    int maxTurn,
    int maxConsecutiveMultiplierCount,
    int mainAttributeIndex,
    List<int> abilityTurnCounts)
    {
        if (abilityTurnCounts.Count != _multiplierAttributes.Count)
            throw new ArgumentException("abilityTurnCounts must match the number of abilities.");

        if (abilityTurnCounts.Sum() != maxTurn)
            throw new ArgumentException("Total turns from abilityTurnCounts must equal maxTurn.");

        const int maxRetries = 100;
        int attempt = 0;

        while (attempt++ < maxRetries)
        {
            multiplierSequence = Enumerable.Repeat(-1, maxTurn).ToList();
            multiplierSequence[0] = mainAttributeIndex;
            multiplierSequence[^1] = mainAttributeIndex;

            List<int> remainingCounts = new List<int>(abilityTurnCounts);
            remainingCounts[mainAttributeIndex] -= 2;

            bool success = true;

            for (int i = 1; i < maxTurn - 1; i++)
            {
                int prev = multiplierSequence[i - 1];
                int consecutiveCount = 1;

                for (int j = i - 2; j >= 0 && multiplierSequence[j] == prev; j--)
                    consecutiveCount++;

                List<int> candidates = new List<int>();
                for (int ability = 0; ability < remainingCounts.Count; ability++)
                {
                    if (remainingCounts[ability] > 0)
                        candidates.Add(ability);
                }

                if (consecutiveCount >= maxConsecutiveMultiplierCount)
                    candidates.Remove(prev);

                if (i == maxTurn - 2)
                    candidates.Remove(mainAttributeIndex);

                if (candidates.Count == 0)
                {
                    success = false;
                    break;
                }

                int choice = candidates[UnityEngine.Random.Range(0, candidates.Count)];
                multiplierSequence[i] = choice;
                remainingCounts[choice]--;
            }

            if (success)
            {
                // Success → raise event
                VBattleRootEventCenter.Instance.Raise(
                    VBattleEventKey.OnMultiplierSequenceCalculated,
                    new Dictionary<string, object>
                    {
                        { "Colors", multiplierSequence.Select(index => _multiplierAttributes[index].color).ToList() }
                    });
                return;
            }
        }

        throw new InvalidOperationException("Unable to generate valid multiplier sequence after retries.");
    }
        

        public void Reset()
        {
            Multiplier = null;
            multiplierSequence.Clear();
            multiplierSequence = null;
            _multiplierAttributes.Clear();
            _multiplierAttributes = null;
        }
    }
    
    public class VBattleAttributeManager
    {
        public Dictionary<string, VBattleAttribute> BattleAttributes => _battleAttributes;
        private Dictionary<string, VBattleAttribute> _battleAttributes;
        
        public VStaminaManager StaminaManager => _staminaManager;
        private VStaminaManager _staminaManager;
        
        public VMultiplierManager MultiplierManager => _multiplierManager;
        private VMultiplierManager _multiplierManager;
        
        private bool _isPhaseEnding;
        
        public VBattleAttributeManager(bool isPhaseEnding)
        {
            _isPhaseEnding = isPhaseEnding;
            _battleAttributes = new Dictionary<string, VBattleAttribute>();
        }

        public void AttributesConversion(VCharacterAttributeManager characterAttributeManager)
        {
            ConvertFromCharacterAttributes(characterAttributeManager);
        }

        public void Clear()
        {
            _battleAttributes.Clear();
            if(_multiplierManager is not null)
                _multiplierManager.Reset();
            _staminaManager.Reset();
        }
        
        public void InitializeInternalManagers(int mainAttributeIndex, List<int> abilityTurnCounts)
        {
            _staminaManager = new VStaminaManager(
                _battleAttributes.TryGetValue("BAStamina", out var stamina) ? (VBattleStaminaAttribute)stamina : null,
                _battleAttributes.TryGetValue("BAShield", out var shield) ? (VBattleStaminaAttribute)shield : null
            );
            
            _multiplierManager = new VMultiplierManager(
                mainAttributeIndex,
                4,
                abilityTurnCounts,
                _battleAttributes.TryGetValue("BASingingMultiplier", out var singing) ? (VBattleMultiplierAttribute)singing : null,
                _battleAttributes.TryGetValue("BAGamingMultiplier", out var gaming) ? (VBattleMultiplierAttribute)gaming : null,
                _battleAttributes.TryGetValue("BAChattingMultiplier", out var chatting) ? (VBattleMultiplierAttribute)chatting : null,
                _battleAttributes.TryGetValue("BATurn", out var turnAttribute) ? (VBattleTurnAttribute)turnAttribute : null
            );

            var viewerCount = _battleAttributes["BAViewerCount"].Value;
            foreach (var multiplier in _multiplierManager.Multipliers)
            {
                multiplier.AddTo(VMathUtils.FloatToInt(viewerCount * 0.1f), false, false);
            }
            
            _multiplierManager.OnEnable();
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnViewerCountChange, OnViewerCountChange);
        }
        
        public void ConvertFromCharacterAttributes(VCharacterAttributeManager characterAttributeManager)
        {
            foreach (var attribute in characterAttributeManager.Attributes)
            {
                if(!attribute.Value.IsConvertToBattleAttribute)
                    continue;
                var battleAttribute = attribute.Value.ConvertToBattleAttribute();
                if (battleAttribute.Value != null)
                {
                    AddAttribute(battleAttribute.Key, battleAttribute.Value);
                    battleAttribute.Value.OnEnable();
                }
            }
        }
        
        public void OnEnable()
        {
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnParameterChange, OnParameterChange);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnTurnEnd, OnTurnEnd);
            
        }
        
        public void OnDisable()
        {
            foreach (var attribute in _battleAttributes)
            {
                attribute.Value.OnDisable();
            }
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnParameterChange, OnParameterChange);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnViewerCountChange, OnViewerCountChange);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnViewerCountChange, OnTurnEnd);
            _multiplierManager.OnDisable();
        }

        private void OnTurnEnd(Dictionary<string, object> messagedict)
        {
            _staminaManager.OnTurnEnd();
        }

        private void OnViewerCountChange(Dictionary<string, object> messagedict)
        {
            var delta = (int)messagedict["Delta"];
            if(delta <= 0)
                return;

            foreach (var multiplier in _multiplierManager.Multipliers)
            {
                multiplier.AddTo(VMathUtils.FloatToInt(delta * 0.1f), false, false);
            }
        }

        private void OnParameterChange(Dictionary<string, object> messagedict)
        {
            if (_battleAttributes.TryGetValue("BAParameter", out var parameter))
            {
                float multiplier = _multiplierManager.Multiplier.Value / 100f;
                int delta = (int)messagedict["Delta"];
                if (delta <= 0)
                    return;
                (_battleAttributes["BAPopularity"] as VBattlePopularityAttribute).
                    AddPopularity((int)(delta * multiplier), MultiplierManager.Multiplier.AttributeName,
                        messagedict["IsFromCard"] as bool? ?? false,
                        messagedict["ShouldPlayTwice"] as bool? ?? false);
            }
        }

        public int PreviewPopularityChange(int delta)
        {
            if (_battleAttributes.TryGetValue("BAParameter", out var parameter))
            {
                float multiplier = _multiplierManager.Multiplier.Value / 100f;
                int parameterDelta = parameter.PreviewAddTo(delta) - parameter.Value;
                return (int)(parameterDelta * multiplier);
            }

            return 0;
        }
        
        public int PreviewShieldChange(int delta)
        {
            if (_battleAttributes.TryGetValue("BAShield", out var parameter))
            {
                int parameterDelta = parameter.PreviewAddTo(delta) - parameter.Value;
                return parameterDelta;
            }

            return 0;
        }

        public bool TryGetAttribute(string name, out VBattleAttribute attribute)
        {
            return _battleAttributes.TryGetValue(name.Trim(), out attribute);
        }
        
        public void AddAttribute(string name, VBattleAttribute attribute)
        {

            _battleAttributes.Add(name, attribute);
            attribute.AttributeName = name;
            attribute.OnEnable();
        }
        
        public void RemoveAttribute(string name)
        {
            if (_battleAttributes.TryGetValue(name, out var attribute))
            {
                attribute.OnDisable();
                _battleAttributes.Remove(name);
            }
        }

        public void SkipTurnRecoverStamina()
        {
            _battleAttributes.TryGetValue("BAStamina", out var stamina);
            _battleAttributes.TryGetValue("BASkipTurnStaminaRecovery", out var recoveryAmount);
            stamina.AddTo(recoveryAmount.Value, false, false);
        }
    }
}