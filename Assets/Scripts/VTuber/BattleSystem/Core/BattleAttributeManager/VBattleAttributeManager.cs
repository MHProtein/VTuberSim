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
    public class VBattleAttributeManagerSaveData
    {
        public List<VBattleAttributeSaveData> attributeSaveDatas;
        public VStaminaManagerSaveData staminaManagerSaveData;
        public VMultiplierManagerSaveData multiplierManagerSaveData;
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
        
        public VBattleAttributeManager(bool isPhaseEnding, VBattleAttributeManagerSaveData saveData)
        {
            if (saveData is not null)
            {
                _isPhaseEnding = isPhaseEnding;
                _battleAttributes = new Dictionary<string, VBattleAttribute>();
                foreach (var attributeSaveData in saveData.attributeSaveDatas)
                {
                    var attribute = Activator.CreateInstance(Type.GetType(attributeSaveData.AttributeType), attributeSaveData) as VBattleAttribute;
                    _battleAttributes.Add(attribute.AttributeName, attribute);
                }
                
                _staminaManager = new VStaminaManager(
                    _battleAttributes.TryGetValue("BAStamina", out var stamina) ? (VBattleStaminaAttribute)stamina : null,
                    _battleAttributes.TryGetValue("BAShield", out var shield) ? (VBattleStaminaAttribute)shield : null,
                saveData.staminaManagerSaveData
                );
                
                _multiplierManager = new VMultiplierManager( 
                    _battleAttributes.TryGetValue("BASingingMultiplier", out var singing) ? (VBattleMultiplierAttribute)singing : null,
                    _battleAttributes.TryGetValue("BAGamingMultiplier", out var gaming) ? (VBattleMultiplierAttribute)gaming : null,
                    _battleAttributes.TryGetValue("BAChattingMultiplier", out var chatting) ? (VBattleMultiplierAttribute)chatting : null,
                    saveData.multiplierManagerSaveData
                );
                
                _multiplierManager.OnEnable();
                return;
            }

            _isPhaseEnding = isPhaseEnding;
            _battleAttributes = new Dictionary<string, VBattleAttribute>();
        }

        public VBattleAttributeManagerSaveData Save()
        {
            var saveData = new VBattleAttributeManagerSaveData();
            saveData.attributeSaveDatas = new List<VBattleAttributeSaveData>();
            foreach (var attribute in _battleAttributes)
            {
                saveData.attributeSaveDatas.Add(attribute.Value.Save());
            }
            saveData.staminaManagerSaveData = _staminaManager.Save();
            saveData.multiplierManagerSaveData = _multiplierManager.Save();
            return saveData;
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
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnTurnEnd, OnTurnEnd);
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
                multiplier.AddTo(VMathUtils.FloatToInt(delta * 0.2f), false, false);
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