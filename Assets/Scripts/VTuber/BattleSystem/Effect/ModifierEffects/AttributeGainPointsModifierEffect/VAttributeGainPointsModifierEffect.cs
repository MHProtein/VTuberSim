using System;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Effect
{
    public class VModifierEffectSaveData
    {
        public uint effectConfigID;
        public int valueModifierID;
        public uint modifierID;
        public bool applied = false;
        public int parameterInt;
        public int upgradedParameterInt;
        public float parameterFloat;
        public float upgradedParameterFloat;
    }
    
    public class VAttributeGainPointsModifierEffect : VModifierEffect
    {
        private string _attributeName;

        private VUpgradableValue<int> _deltaPoints;

        private int _valueModifierID = -1;
        private Action<uint> _onBuffRemove;
        private Action<uint, float> _onBuffLayerChangeRate;
        private Action<uint, int> _onBuffLayerChangePoints;
        private uint _modifierID;
        private bool _applied = false;
        
        public VAttributeGainPointsModifierEffect(VAttributeGainPointsModifierEffectConfiguration configuration, string parameter, string upgradedParameter) : base(configuration)
        {
            _attributeName = configuration.attributeName;
            _deltaPoints = new VUpgradableValue<int>(Convert.ToInt32(parameter), Convert.ToInt32(upgradedParameter));
        }
        
        public override VModifierEffectSaveData Save()
        {
            return new VModifierEffectSaveData
            {
                effectConfigID = _configuration.id,
                valueModifierID = _valueModifierID,
                modifierID = _modifierID,
                applied = _applied,
                parameterInt = _deltaPoints.Value,
                upgradedParameterInt = _deltaPoints.UpgradedValue,
            };
        }

        public override void Load(VModifierEffectSaveData data)
        {
            _deltaPoints = new VUpgradableValue<int>(data.parameterInt, data.upgradedParameterInt);
            if (data.applied)
            {
                _applied = true;
                _valueModifierID = data.valueModifierID;
                _modifierID = data.modifierID;

                var modifier = VBattleLookUpTables.Instance.GetGainValueModifier(_valueModifierID);
                _onBuffRemove = modifier.RemoveModifier;
                _onBuffLayerChangePoints = modifier.ChangeModifier;
                VDebug.Log("效果 " + _configuration.effectName + " 添加了 " + _deltaPoints.Value + " 获取Points Modifier，ID为: " + _modifierID);
            }
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
            _battle = battle;
            Apply(battle, layer);
        }

        public override void OnBuffLayerChange(int layer)
        {
            if (!_applied)
            {
                Apply(_battle, layer);
                return;
            }
            if (MultiplyByLayer < 0.0f)
                return;

            float pointValue = _deltaPoints.Value;
            pointValue *= layer * MultiplyByLayer;
            _onBuffLayerChangePoints(_modifierID, (int)pointValue);
            VDebug.Log("效果 " + _configuration.effectName + " 将额外获取点数修改为 " + pointValue + "，层数为 " + layer);
        }

        public override void OnBuffRemove()
        {
            if (_onBuffRemove is null)
            {
                VDebug.LogError("OnBuffRemove 为 null，_modifierID: " + _modifierID + "，属性: " + _attributeName + "，请检查属性名");
                return;
            }
            _onBuffRemove(_modifierID);
            VDebug.Log("效果 " + _configuration.effectName + " 移除了获取Points Modifier，ID为: " + _modifierID);
        }
        public override string GetValue()
        {
            return _deltaPoints.Value.ToString();
        }

        public void Apply(VBattle battle, int layer)
        {
            if (_applied)
                return;
            if (!CanApply(battle, null))
                return;
            if(battle.BattleAttributeManager.TryGetAttribute(_attributeName, out var attribute))
            {
                Triggered = true;
                float pointValue = _deltaPoints.Value;
                if(MultiplyByLayer > 0.0f)
                    pointValue *= layer * MultiplyByLayer;

                _valueModifierID = attribute.GainPointsModifier.ID;
                _modifierID = attribute.GainPointsModifier.AddModifier((int)pointValue, -1);
                _onBuffRemove = attribute.GainPointsModifier.RemoveModifier;
                _onBuffLayerChangePoints = attribute.GainPointsModifier.ChangeModifier;
                VDebug.Log("效果 " + _configuration.effectName + " 添加了 " + _deltaPoints.Value + " 获取Points Modifier，ID为: " + _modifierID);
            }
            else
            {
                VDebug.LogError(_attributeName + "not found");
            }
        }
    }
}