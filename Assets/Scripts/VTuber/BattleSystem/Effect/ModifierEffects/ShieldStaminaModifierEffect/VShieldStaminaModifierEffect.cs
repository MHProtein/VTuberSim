using System;
using System.Globalization;
using VTuber.BattleSystem.BattleAttribute;
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
    public class VShieldStaminaModifierEffect : VEffect
    {
        private readonly VStaminaModifiyType _modifiyType;
        private readonly VUpgradableValue<float> _deltaRate;
        private readonly VUpgradableValue<int> _deltaPoints;
        
        private Action<uint> _onBuffRemove;
        private Action<uint, float> _onBuffLayerChangeRate;
        private Action<uint, int> _onBuffLayerChangePoints;
        
        private uint modifierID;
        
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

        public override void OnBuffAdded(VBattle battle, int layer)
        {            
            switch (_modifiyType)
            {
                case VStaminaModifiyType.Rate:
                    float rateValue = _deltaRate.Value;
                    if(MultiplyByLayer > 0.0f)
                        rateValue *= layer * MultiplyByLayer;
            
                    modifierID = battle.BattleAttributeManager.StaminaManager.ConsumeRateModifier.AddModifier(rateValue, -1);
                    _onBuffRemove = battle.BattleAttributeManager.StaminaManager.ConsumeRateModifier.RemoveModifier;
                    _onBuffLayerChangeRate = battle.BattleAttributeManager.StaminaManager.ConsumeRateModifier.ChangeModifier;
                    VDebug.Log($"效果 {_configuration.effectName} 添加了 {_deltaRate.Value} 获取RateModifier，ID：{modifierID}");
                
                    break;
                case VStaminaModifiyType.Points:
                    int pointsValue = _deltaPoints.Value;
                    if(MultiplyByLayer > 0.0f)
                        pointsValue *= VMathUtils.FloatToInt(layer * MultiplyByLayer);
            
                    modifierID = battle.BattleAttributeManager.StaminaManager.ConsumePointsModifier.AddModifier(pointsValue, -1);
                    _onBuffRemove = battle.BattleAttributeManager.StaminaManager.ConsumePointsModifier.RemoveModifier;
                    _onBuffLayerChangePoints = battle.BattleAttributeManager.StaminaManager.ConsumePointsModifier.ChangeModifier;
                    VDebug.Log($"效果 {_configuration.effectName} 添加了 {_deltaPoints.Value} 获取PointsModifier，ID：{modifierID}");
                    break;
            }
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
                    _onBuffLayerChangeRate(modifierID, rateValue);
                    VDebug.Log($"效果 {_configuration.effectName} 修改了RateModifier为 {rateValue}，层数：{layer}");
                    break;
                case VStaminaModifiyType.Points:
                    if (MultiplyByLayer < 0.0f)
                        return;
                    int pointsValue = _deltaPoints.Value;
                    pointsValue *= VMathUtils.FloatToInt(layer * MultiplyByLayer);
                    _onBuffLayerChangePoints(modifierID, pointsValue);
                    VDebug.Log($"效果 {_configuration.effectName} 修改了PointsModifier为 {pointsValue}，层数：{layer}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        public override void OnBuffRemove()
        {
            _onBuffRemove(modifierID);
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