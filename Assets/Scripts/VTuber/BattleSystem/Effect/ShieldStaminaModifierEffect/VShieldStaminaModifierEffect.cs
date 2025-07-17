using System;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Effect.ShieldStaminaModifierEffect
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
        
        
        private Action<uint> _onBuffRemoveShield;
        private Action<uint, float> _onBuffLayerChangeRateShield;
        private Action<uint, int> _onBuffLayerChangePointsShield;
        
        private uint modifierID;
        private uint modifierIDShield;
        
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
            _deltaRate.Upgrade();
        }
        
        public override void Downgrade()
        {
            base.Downgrade();
            _deltaRate.Downgrade();
        }

        public override void OnBuffAdded(VBattle battle, int layer)
        {
            AddShieldModifier(battle, layer);
            AddStaminaModifier(battle, layer);
        }
        
        public void AddShieldModifier(VBattle battle, int layer)
        {
            switch (_modifiyType)
            {
                case VStaminaModifiyType.Rate:
                    if(battle.BattleAttributeManager.TryGetAttribute("BAShield", out var attribute))
                    {
                        float rateValue = _deltaRate.Value;
                        if(MultiplyByLayer > 0.0f)
                            rateValue *= layer * MultiplyByLayer;
                
                        modifierIDShield = attribute.GainRateModifier.AddModifier(rateValue);
                        _onBuffRemoveShield = attribute.GainRateModifier.RemoveModifier;
                        _onBuffLayerChangeRateShield = attribute.GainRateModifier.ChangeModifier;
                        VDebug.Log("Effect " + _configuration.effectName + " added " + _deltaRate.Value + " gain rate modifier with ID: " + modifierIDShield);
                    }
                    break;
                case VStaminaModifiyType.Points:
                    if(battle.BattleAttributeManager.TryGetAttribute("BAShield", out var attributeShield))
                    {
                        int pointsValue = _deltaPoints.Value;
                        if(MultiplyByLayer > 0.0f)
                            pointsValue *= (int)(layer * MultiplyByLayer);
                
                        modifierIDShield = attributeShield.GainPointsModifier.AddModifier(pointsValue);
                        _onBuffRemoveShield = attributeShield.GainPointsModifier.RemoveModifier;
                        _onBuffLayerChangePointsShield = attributeShield.GainPointsModifier.ChangeModifier;
                        VDebug.Log("Effect " + _configuration.effectName + " added " + _deltaPoints.Value + " gain points modifier with ID: " + modifierIDShield);
                    }
                    break;
            }

        }
        
        public void AddStaminaModifier(VBattle battle, int layer)
        {
            switch (_modifiyType)
            {
                case VStaminaModifiyType.Rate:
                    if(battle.BattleAttributeManager.TryGetAttribute("BAStamina", out var attribute))
                    {
                        float rateValue = _deltaRate.Value;
                        if(MultiplyByLayer > 0.0f)
                            rateValue *= layer * MultiplyByLayer;
                
                        modifierID = attribute.GainRateModifier.AddModifier(rateValue);
                        _onBuffRemove = attribute.GainRateModifier.RemoveModifier;
                        _onBuffLayerChangeRate = attribute.GainRateModifier.ChangeModifier;
                        VDebug.Log("Effect " + _configuration.effectName + " added " + _deltaRate.Value + " gain rate modifier with ID: " + modifierID);
                    }
                    break;
                case VStaminaModifiyType.Points:
                    if(battle.BattleAttributeManager.TryGetAttribute("BAStamina", out var attributeStamina))
                    {
                        int pointsValue = _deltaPoints.Value;
                        if(MultiplyByLayer > 0.0f)
                            pointsValue *= (int)(layer * MultiplyByLayer);
                
                        modifierID = attributeStamina.GainPointsModifier.AddModifier(pointsValue);
                        _onBuffRemove = attributeStamina.GainPointsModifier.RemoveModifier;
                        _onBuffLayerChangePoints = attributeStamina.GainPointsModifier.ChangeModifier;
                        VDebug.Log("Effect " + _configuration.effectName + " added " + _deltaPoints.Value + " gain points modifier with ID: " + modifierID);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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
                    _onBuffLayerChangeRateShield(modifierIDShield, rateValue);
                    VDebug.Log("Effect " + _configuration.effectName + " changed gain rate to " + rateValue + " for layer " + layer);
                    break;
                case VStaminaModifiyType.Points:
                    if (MultiplyByLayer < 0.0f)
                        return;
                    int pointsValue = _deltaPoints.Value;
                    pointsValue *= (int)(layer * MultiplyByLayer);
                    _onBuffLayerChangePoints(modifierID, pointsValue);
                    _onBuffLayerChangePointsShield(modifierIDShield, pointsValue);
                    VDebug.Log("Effect " + _configuration.effectName + " changed gain points to " + pointsValue + " for layer " + layer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        public override void OnBuffRemove()
        {
            _onBuffRemove(modifierID);
            _onBuffRemoveShield(modifierIDShield);
        }
    }
}