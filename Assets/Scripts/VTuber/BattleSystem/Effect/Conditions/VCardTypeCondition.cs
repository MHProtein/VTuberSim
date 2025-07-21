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
                VDebug.Log($"条件 {id} 未通过：消息中未找到 'Card' 键。");
                return false;
            }
            bool result = _targetValue.Equals(((VCard)message["Card"]).CardType);
            if (result)
            {
                VDebug.Log($"条件 {id} 通过：卡牌类型为 {_targetValue}");
            }
            else
            {
                VDebug.Log($"条件 {id} 未通过：卡牌类型不为 {_targetValue}");
            }
            return result;
        }
    }
}