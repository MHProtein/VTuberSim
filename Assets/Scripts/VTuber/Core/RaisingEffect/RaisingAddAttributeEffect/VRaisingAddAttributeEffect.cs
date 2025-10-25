using System.Collections.Generic;
using VTuber.BattleSystem.Effect;
using VTuber.Character;
using VTuber.Core.Foundation;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddAttributeEffect : VRaisingEffect, IAttributeEffect
    {
        private readonly VUpgradableValue<int> _value;

        public VRaisingAddAttributeEffect(VRaisingAddAttributeEffectConfiguration configuration, int value,
            int upgradedValue) : base(configuration)
        {
            AttributeName = configuration.AbilityName;
            _value = new VUpgradableValue<int>(value, upgradedValue);
        }

        public string AttributeName { get; }

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict)
        {
            if (character.AttributeManager.TryGetAttribute(AttributeName, out var attribute))
            {
                attribute.AddTo(_value.Value, true);
                VDebug.Log("Added " + _value + " To " + AttributeName);
            }
        }

        public override void Upgrade()
        {
            _value.Upgrade();
        }

        public override void DownGrade()
        {
            _value.Downgrade();
        }

        public override string GetParameter()
        {
            return _value.Value.ToString();
        }
    }
}