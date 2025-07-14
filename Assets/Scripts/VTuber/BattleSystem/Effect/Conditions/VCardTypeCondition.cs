using System.Collections.Generic;
using CsvHelper;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.Effect.Conditions
{
    public class VCardTypeCondition : VEffectCondition
    {
        private string _targetValue;

        public VCardTypeCondition(CsvReader csv) : base(csv)
        {
            _targetValue = csv.GetField<string>("TargetValue");
        }

        public override bool IsTrue(VBattle battle, Dictionary<string, object> message)
        {
            return _targetValue.Equals(((VCard)message["Card"]).CardType);
        }
    }
}