using System.Collections.Generic;
using VTuber.BattleSystem.Card;
using VTuber.Character.Attribute;
using VTuber.Core.EventCenter;
using VTuber.Core.RaisingEffect;
using VTuber.Core.UI;
using VTuber.RaisingAnimationSystem;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.Character.Attributes
{
    public class VPressureAttribute : VCharacterAttribute
    {
        private readonly List<VRaisingEffect> _effects;

        public VPressureAttribute(VCharacterAttributeConfiguration configuration, List<VEffectItem> effects,
            int initialValue,
            VRaisingEventKey eventKey = VRaisingEventKey.Default,
            int maxValue = int.MaxValue, int minValue = 0, bool isPercentage = false)
            : base(configuration, initialValue, eventKey, maxValue, minValue, isPercentage, false , false)
        {
            _effects = new List<VRaisingEffect>();
            foreach (var effect in effects) _effects.Add(effect.CreateRaisingEffect());
            SetValue(initialValue, false);
        }

        public void ApplyEffects(VCharacter character)
        {
            if (Value == 3)
                return;
            _effects[Value - 1].ApplyEffect(character, null, VAnimationRequestFactory.Create(VInstigatorType.Pressure,
                VUIUtils.Instance.GetPressureIcon(Value).Value, "每天结束时, " + _effects[Value - 1].Description));
        }
        
        protected override void AddAdditionalEventParameters(Dictionary<string, object> messageDict)
        {
            messageDict.Add("Effect", _effects[Value - 1]);
        }
    }
}