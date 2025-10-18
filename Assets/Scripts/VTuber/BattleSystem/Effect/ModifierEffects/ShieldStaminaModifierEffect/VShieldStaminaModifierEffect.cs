using System;
using System.Globalization;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Buff;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;
using VTuber.Core.UI;

namespace VTuber.BattleSystem.Effect
{
    public enum VStaminaModifiyType
    {
        Rate,
        Points
    }
    public class VShieldStaminaModifierEffect : VModifierEffect
    {
        private readonly VStaminaModifiyType _modifiyType;
        private VUpgradableValue<float> _deltaRate;
        private VUpgradableValue<int> _deltaPoints;
        
        private int _valueModifierID = -1;
        private Action<uint> _onBuffRemove;
        private Action<uint, float> _onBuffLayerChangeRate;
        private Action<uint, int> _onBuffLayerChangePoints;
        
        private uint _modifierID;
        private bool _applied = false;
        
        public VShieldStaminaModifierEffect(VShieldStaminaModifierEffectConfiguration configuration, string parameter, string upgradedParameter) : base(configuration)
        {
            _modifiyType = configuration.modifyType;

            switch (_modifiyType)
            {
                case VStaminaModifiyType.Rate:
                    _deltaRate = new VUpgradableValue<float>(Convert.ToSingle(parameter), Convert.ToSingle(upgradedParameter));
                    break;
                case VStaminaModifiyType.Points:
                    _deltaPoints = new VUpgradableValue<int>(Convert.ToInt32(parameter), Convert.ToInt32(upgradedParameter));
                    break;
            }
        }
        
        public override VModifierEffectSaveData Save()
        {
            var saveData = new VModifierEffectSaveData
            {
                effectConfigID = _configuration.id,
                valueModifierID = _valueModifierID,
                modifierID = _modifierID,
                applied = _applied,
            };
            if(_modifiyType == VStaminaModifiyType.Points){
                saveData.parameterInt = _deltaPoints.Value;
                saveData.upgradedParameterInt = _deltaPoints.UpgradedValue;
            }
            else if(_modifiyType == VStaminaModifiyType.Rate){
                saveData.parameterFloat = _deltaRate.Value;
                saveData.upgradedParameterFloat = _deltaRate.UpgradedValue;
            }
            return saveData;
        }

        public override void Load(VModifierEffectSaveData data)
        {
            switch (_modifiyType)
            {
                case VStaminaModifiyType.Rate:
                    _deltaRate = new VUpgradableValue<float>(data.parameterFloat, data.upgradedParameterFloat);
                    break;
                case VStaminaModifiyType.Points:
                    _deltaPoints = new VUpgradableValue<int>(data.parameterInt, data.upgradedParameterInt);
                    break;
            }
            if (data.applied)
            {
                _applied = true;
                _valueModifierID = data.valueModifierID;
                _modifierID = data.modifierID;

                switch (_modifiyType)
                {
                    case VStaminaModifiyType.Rate:
                    {
                        var modifier = VBattleLookUpTables.Instance.GetGainRateModifier(_valueModifierID);
                        _onBuffLayerChangeRate = modifier.ChangeModifier;
                        _onBuffRemove = (id) =>
                        {
                            modifier.RemoveModifier(id);
                            modifier.onModifierApply -= NotifyBuffItemEffectApply;
                        };
                        modifier.onModifierApply += NotifyBuffItemEffectApply;
                        VDebug.Log("效果 " + _configuration.effectName + " 添加了 " + _deltaPoints.Value + " 获取Rate Modifier，ID为: " + _modifierID);
                        break;
                    }
                    case VStaminaModifiyType.Points:
                    {
                        
                        var modifier = VBattleLookUpTables.Instance.GetGainValueModifier(_valueModifierID);
                        _onBuffLayerChangePoints = modifier.ChangeModifier;
                        _onBuffRemove = (id) =>
                        {
                            modifier.RemoveModifier(id);
                            modifier.onModifierApply -= NotifyBuffItemEffectApply;
                        };
                        modifier.onModifierApply += NotifyBuffItemEffectApply;
                        VDebug.Log("效果 " + _configuration.effectName + " 添加了 " + _deltaPoints.Value + " 获取Points Modifier，ID为: " + _modifierID);
                        break;
                    }
                }
            }
        }

        public override void Upgrade()
        {
            base.Upgrade();        
            switch (_modifiyType)
            {
                case VStaminaModifiyType.Rate:
                    _deltaRate.Upgrade();
                    break;
                case VStaminaModifiyType.Points:
                    _deltaPoints.Upgrade();
                    break;
            }
        }
        
        public override void Downgrade()
        {
            base.Downgrade();
            switch (_modifiyType)
            {
                case VStaminaModifiyType.Rate:
                    _deltaRate.Downgrade();
                    break;
                case VStaminaModifiyType.Points:
                    _deltaPoints.Downgrade();
                    break;
            }
        }

        public override void OnBuffAdded(VBattle battle, int layer, VBuffItem buffItem)
        {            
            _battle = battle;
            _buffItem = buffItem;
            Apply(battle, layer);
        }

        private void Apply(VBattle battle, int layer)
        {       
            if (_applied)
                return;
            if (!CanApply(battle, null))
                return;
            switch (_modifiyType)
            {
                case VStaminaModifiyType.Rate:
                    float rateValue = _deltaRate.Value;
                    if(MultiplyByLayer > 0.0f)
                        rateValue *= layer * MultiplyByLayer;
            
                    _valueModifierID = battle.BattleAttributeManager.StaminaManager.ConsumeRateModifier.ID;
                    _modifierID = battle.BattleAttributeManager.StaminaManager.ConsumeRateModifier.AddModifier(rateValue, -1);
                    _onBuffRemove = (id)=>
                    {
                        battle.BattleAttributeManager.StaminaManager.ConsumeRateModifier.RemoveModifier(id);
                        battle.BattleAttributeManager.StaminaManager.ConsumeRateModifier.onModifierApply -= NotifyBuffItemEffectApply;
                    };
                    battle.BattleAttributeManager.StaminaManager.ConsumeRateModifier.onModifierApply += NotifyBuffItemEffectApply;
                    _onBuffLayerChangeRate = battle.BattleAttributeManager.StaminaManager.ConsumeRateModifier.ChangeModifier;
                    VDebug.Log($"效果 {_configuration.effectName} 添加了 {_deltaRate.Value} 获取RateModifier，ID：{_modifierID}");
                
                    break;
                case VStaminaModifiyType.Points:
                    int pointsValue = _deltaPoints.Value;
                    if(MultiplyByLayer > 0.0f)
                        pointsValue *= VMathUtils.FloatToInt(layer * MultiplyByLayer);
            
                    _valueModifierID = battle.BattleAttributeManager.StaminaManager.ConsumePointsModifier.ID;
                    _modifierID = battle.BattleAttributeManager.StaminaManager.ConsumePointsModifier.AddModifier(pointsValue, -1);
                    _onBuffRemove = (id) =>
                    {
                        battle.BattleAttributeManager.StaminaManager.ConsumePointsModifier.RemoveModifier(id);
                        battle.BattleAttributeManager.StaminaManager.ConsumePointsModifier.onModifierApply -= NotifyBuffItemEffectApply;
                    };
                    _onBuffLayerChangePoints = battle.BattleAttributeManager.StaminaManager.ConsumePointsModifier.ChangeModifier;
                    battle.BattleAttributeManager.StaminaManager.ConsumePointsModifier.onModifierApply += NotifyBuffItemEffectApply;
                    VDebug.Log($"效果 {_configuration.effectName} 添加了 {_deltaPoints.Value} 获取PointsModifier，ID：{_modifierID}");
                    break;
            }
            Triggered = true;
            _applied = true;
        }
        


        public override void OnBuffLayerChange(int layer)
        {
            switch (_modifiyType)
            {
                case VStaminaModifiyType.Rate:
                    
                    if (MultiplyByLayer < 0.0f)
                        return;
                    float rateValue = _deltaRate.Value;
                    rateValue *= layer * MultiplyByLayer;
                    _onBuffLayerChangeRate(_modifierID, rateValue);
                    VDebug.Log($"效果 {_configuration.effectName} 修改了RateModifier为 {rateValue}，层数：{layer}");
                    break;
                case VStaminaModifiyType.Points:
                    if (MultiplyByLayer < 0.0f)
                        return;
                    int pointsValue = _deltaPoints.Value;
                    pointsValue *= VMathUtils.FloatToInt(layer * MultiplyByLayer);
                    _onBuffLayerChangePoints(_modifierID, pointsValue);
                    VDebug.Log($"效果 {_configuration.effectName} 修改了PointsModifier为 {pointsValue}，层数：{layer}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        public override void OnBuffRemove()
        {
            _onBuffRemove(_modifierID);
        }
        
        public override string GetValue()
        {
            switch (_modifiyType)
            {
                case VStaminaModifiyType.Rate:
                    return VMathUtils.FloatToInt(_deltaRate.Value * 100) + "%";
                case VStaminaModifiyType.Points:
                    return _deltaPoints.Value.ToString();
            }

            return "";
        }
    }
}