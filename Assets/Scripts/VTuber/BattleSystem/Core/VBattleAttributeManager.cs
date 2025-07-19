using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Buff;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

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
        
        public void ApplyCost(int cost)
        {
            int calculatedCost = CalculateCost(cost);

            int costAfterShield = calculatedCost - _staminaAttribute.Value;

            _shieldAttribute.AddTo(-calculatedCost, false);
            if (costAfterShield <= 0)
                return;
            
            _staminaAttribute.AddTo(-costAfterShield, false);
        }

        public bool TestCost(int cost)
        {
            int calculatedCost = CalculateCost(cost);
            
            int costAfterShield = calculatedCost - _shieldAttribute.Value;
            
            if (costAfterShield <= 0)
                return true;
            
            return _staminaAttribute.TestCost(-costAfterShield);
        }
        
        public int CalculateCost(int delta)
        {
            delta = (int)(delta * (1.0f - VValueModifier<int>.GetModifierFloatValue(consumeRateModifier)))
                    - VValueModifier<int>.GetModifierIntValue(consumePointsModifier);

            return delta;
        }
        
    }

    public class VMultiplierManager
    {
        private List<VBattleMultiplierAttribute> _multiplierAttributes;
        private List<int> multiplierSequence;
        public VMultiplierManager(int mainAttributeIndex, 
            int maxConsecutiveMultiplierCount, 
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
            ++mainAttributeIndex;
            multiplierSequence[0] = mainAttributeIndex;
            multiplierSequence[^1] = mainAttributeIndex;

            GenerateMultiplierSequence(turnAttribute.MaxTurn, maxConsecutiveMultiplierCount, mainAttributeIndex);
        }
        
        public void OnEnable()
        {
            
        }
            
        public void OnDisable()
        {
                
        }

        private void GenerateMultiplierSequence(int maxTurn, int maxConsecutiveMultiplierCount, int mainAttributeIndex)
        {
            int consecutiveMultiplierCount = 1;
            for (int i = 1; i < maxTurn - 2; i++)
            {
                int index = Random.Range(1, 4);
                
                if (index == multiplierSequence[i - 1])
                    ++consecutiveMultiplierCount;
                else
                    consecutiveMultiplierCount = 1;
                
                if (consecutiveMultiplierCount >= maxConsecutiveMultiplierCount)
                {
                    while (index == multiplierSequence[i - 1] || 
                           (i == multiplierSequence.Count - 3 && index == mainAttributeIndex))
                    {
                        index = Random.Range(1, 4);
                    }
                }
                else if (i == multiplierSequence.Count - 3)
                {
                    while (index == mainAttributeIndex)
                    {
                        index = Random.Range(1, 4);
                    }
                }
                
                multiplierSequence[i] = index;
            }
            if(multiplierSequence[^1] + multiplierSequence[^3] == 3)
                multiplierSequence[^2] = 3;
            if (multiplierSequence[^1] + multiplierSequence[^3] == 4)
                multiplierSequence[^2] = 2;
            if (multiplierSequence[^1] + multiplierSequence[^3] == 5)
                multiplierSequence[^2] = 1;
            multiplierSequence = multiplierSequence.Select(x=> x - 1).ToList();
            
            string logString = "Multiplier Sequence: ";

            foreach (var index in multiplierSequence)
            {
                logString += _multiplierAttributes[index].AttributeName + " => ";
            }

            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnMultiplierSequenceCalculated,
                new Dictionary<string, object>()
                {
                    {"Colors", multiplierSequence.Select(index => _multiplierAttributes[index].color).ToList()},
                });
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
        
        public VBattleAttributeManager(VCharacterAttributeManager characterAttributeManager)
        {
            _battleAttributes = new Dictionary<string, VBattleAttribute>();
            ConvertFromCharacterAttributes(characterAttributeManager);
        }

        public void InitializeInternalManagers()
        {
            _staminaManager = new VStaminaManager(
                _battleAttributes.TryGetValue("BAStamina", out var stamina) ? (VBattleStaminaAttribute)stamina : null,
                _battleAttributes.TryGetValue("BAShield", out var shield) ? (VBattleStaminaAttribute)shield : null
            );
            _multiplierManager = new VMultiplierManager(
                0,
                4,
                _battleAttributes.TryGetValue("BASingingMultiplier", out var singing) ? (VBattleMultiplierAttribute)singing : null,
                _battleAttributes.TryGetValue("BAGamingMultiplier", out var gaming) ? (VBattleMultiplierAttribute)gaming : null,
                _battleAttributes.TryGetValue("BAChattingMultiplier", out var chatting) ? (VBattleMultiplierAttribute)chatting : null,
                _battleAttributes.TryGetValue("BATurn", out var turnAttribute) ? (VBattleTurnAttribute)turnAttribute : null
            );
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
                }
            }
        }
        
        public void OnEnable()
        {
            foreach (var attribute in _battleAttributes)
            {
                attribute.Value.OnEnable();
            }
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnParameterChange, OnParameterChange);
        }

        private void OnParameterChange(Dictionary<string, object> messagedict)
        {
            if (_battleAttributes.TryGetValue("BAParameter", out var parameter))
            {
                float multiplier = _battleAttributes["BASingingMultiplier"].Value / 100f;
                int delta = (int)messagedict["Delta"];
                _battleAttributes["BAPopularity"].AddTo((int)(delta * multiplier), false);
            }
        }

        public bool TryGetAttribute(string name, out VBattleAttribute attribute)
        {
            return _battleAttributes.TryGetValue(name, out attribute);
        }
        
        public void OnDisable()
        {
            foreach (var attribute in _battleAttributes)
            {
                attribute.Value.OnDisable();
            }
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnParameterChange, OnParameterChange);
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
    }
}