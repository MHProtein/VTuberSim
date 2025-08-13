using System.Collections.Generic;
using Spire.Xls;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.Effect.Conditions
{
    public class VCardTypeUsedCountCondition : VEffectCondition
    {
        
        private string _cardType;
        private int _targetValue;

        public VCardTypeUsedCountCondition(CellRange row) : base(row)
        {
            _cardType = row.Columns[VConditionHeaderIndex.NameOrID].Value;
            _targetValue = ToInt( row.Columns[VConditionHeaderIndex.TargetValue].Value);
        }

        public override bool IsTrue(VBattle battle, Dictionary<string, object> message)
        {
            if ((message["Card"] as VCard).CardType != _cardType)
                return false;
            if (battle.CardTypeHistory.TryGetValue(_cardType, out var count))
            {
                if (count % _targetValue == 0)
                {
                    return true;
                }

                return false;
            }
            return false;
        }
    }
}