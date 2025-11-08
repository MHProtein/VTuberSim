using System.Collections.Generic;
using VTuber.BattleSystem.Card;
using VTuber.Character.Attribute;
using VTuber.Core.EventCenter;
using VTuber.Core.RaisingEffect;

namespace VTuber.Character.Attributes
{
    public class VPressureAttribute : VCharacterAttribute
    {
        private readonly List<VRaisingEffect> _effects;

        public VPressureAttribute(VCharacterAttributeConfiguration configuration, List<VEffectItem> effects,
            int initialValue,
            VRaisingEventKey eventKey = VRaisingEventKey.Default,
            int maxValue = int.MaxValue, int minValue = 0, bool isPercentage = false)
            : base(configuration, initialValue, eventKey, maxValue, minValue, isPercentage, false)
        {
            _effects = new List<VRaisingEffect>();
            foreach (var effect in effects) _effects.Add(effect.CreateRaisingEffect());
        }

        public void ApplyEffects(VCharacter character)
        {
            _effects[Value - 1].ApplyEffect(character, null, VInstigatorType.Pressure, null);
        }
    }
}