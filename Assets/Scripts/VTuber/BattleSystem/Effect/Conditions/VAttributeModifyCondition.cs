using System.Collections.Generic;
using CsvHelper;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.Effect.Conditions
{
    public class VAttributeModifyCondition : VEffectCondition
    {
        private int _targetValue;
        private int _targetDelta;

        public VAttributeModifyCondition(CsvReader csv) : base(csv)
        {
            _targetValue = csv.GetField<int>("TargetValue");
            _targetDelta = csv.GetField<int>("TargetDelta");
        }

        public override bool IsTrue(VBattle battle, Dictionary<string, object> message)
        {
            return Compare((int)message["NewValue"], _targetValue) && Compare((int)message["Delta"], _targetDelta);
        }
    }
}