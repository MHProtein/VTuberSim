using System.Collections.Generic;
using Spire.Xls;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;

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
            if (!message.ContainsKey("Card"))
            {
                VDebug.Log("Condition " + id + " failed: 'Card' key not found in message.");
                return false; // Ensure the message contains a valid card
            }
            bool result = _targetValue.Equals(((VCard)message["Card"]).CardType);
            if (result)
            {
                VDebug.Log("Condition " + id + " passed: Card type matches " + _targetValue);
            }
            else
            {
                VDebug.Log("Condition " + id + " failed: Card type does not match " + _targetValue);
            }
            return result;
        }
    }
}