using System.Collections.Generic;
using Spire.Xls;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.Effect.Conditions
{
    public class VCardTypeCondition : VEffectCondition
    {
        private string _targetValue;

        public VCardTypeCondition(CellRange row) : base(row)
        {
            _targetValue = row.Columns[VConditionHeaderIndex.TargetValue].Value;
        }

        public override bool IsTrue(VBattle battle, Dictionary<string, object> message)
        {
            return _targetValue.Equals(((VCard)message["Card"]).CardType);
        }
    }
}