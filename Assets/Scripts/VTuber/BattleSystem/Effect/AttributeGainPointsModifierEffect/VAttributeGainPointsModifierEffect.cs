using System;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Effect
{
    public class VAttributeGainPointsModifierEffect : VEffect
    {
        private string _attributeName;

        private readonly VUpgradableValue<int> _deltaPoints;

        private Action<uint> _onBuffRemove;
        private Action<uint, float> _onBuffLayerChangeRate;
        private Action<uint, int> _onBuffLayerChangePoints;
        private uint modifierID;
        
        public VAttributeGainPointsModifierEffect(VAttributeGainPointsModifierEffectConfiguration configuration, string parameter, string upgradedParameter) : base(configuration)
        {
            _attributeName = configuration.attributeName;
            _deltaPoints = new VUpgradableValue<int>(Convert.ToInt32(parameter), Convert.ToInt32(upgradedParameter));
        }
        
        public override void Upgrade()
        {
            base.Upgrade();
            _deltaPoints.Upgrade();
        }
        
        public override void Downgrade()
        {
            base.Downgrade();
            _deltaPoints.Downgrade();
        }
        
        public override void OnBuffAdded(VBattle battle, int layer)
        {
            if(battle.BattleAttributeManager.TryGetAttribute(_attributeName, out var attribute))
            {
                float pointValue = _deltaPoints.Value;
                if(MultiplyByLayer > 0.0f)
                    pointValue *= layer * MultiplyByLayer;
                
                modifierID = attribute.GainPointsModifier.AddModifier((int)pointValue);
                _onBuffRemove = attribute.GainPointsModifier.RemoveModifier;
                _onBuffLayerChangePoints = attribute.GainPointsModifier.ChangeModifier;
                VDebug.Log("效果 " + _configuration.effectName + " 添加了 " + _deltaPoints.Value + " 获取Points Modifier，ID为: " + modifierID);
            }
        }

        public override void OnBuffLayerChange(int layer)
        {
            if (MultiplyByLayer < 0.0f)
                return;

            float pointValue = _deltaPoints.Value;
            pointValue *= layer * MultiplyByLayer;
            _onBuffLayerChangePoints(modifierID, (int)pointValue);
            VDebug.Log("效果 " + _configuration.effectName + " 将额外获取点数修改为 " + pointValue + "，层数为 " + layer);
        }

        public override void OnBuffRemove()
        {
            if (_onBuffRemove is null)
            {
                VDebug.LogError("OnBuffRemove 为 null，modifierID: " + modifierID + "，属性: " + _attributeName + "，请检查属性名");
                return;
            }
            _onBuffRemove(modifierID);
            VDebug.Log("效果 " + _configuration.effectName + " 移除了获取Points Modifier，ID为: " + modifierID);
        }
    }
}