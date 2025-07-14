using System.Collections.Generic;
using CsvHelper;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.Effect.Conditions
{
    public class VBuffModifyCondition : VEffectCondition
    {
        private int _buffId;
        private int _targetValue;
        private int _targetDelta;


        public VBuffModifyCondition(CsvReader csv) : base(csv)
        {
            _buffId = csv.GetField<int>("BuffId");
            _targetValue = csv.GetField<int>("TargetValue");
            _targetDelta = csv.GetField<int>("TargetDelta");
        }

        public override bool IsTrue(VBattle battle, Dictionary<string, object> message)
        {
            if ((int)message["BuffId"] != _buffId)
                return false;

            return Compare((int)message["Value"], _targetValue) && Compare((int)message["Delta"], _targetDelta);
        }
    }
}