using System;
using System.Collections.Generic;
using Spire.Xls;
using VTuber.BattleSystem.Card;
using VTuber.Character;

namespace VTuber.Relic
{
    public class VCardOperationRelicCondition : VRaisingRelicCondition
    {
        public enum VCardAttributeType
        {
            Type,
            Rarity
        }

        private VCardAttributeType _cardAttributeType;
        private string _targetValue;

        public VCardOperationRelicCondition(CellRange row) : base(row)
        {
            
            _cardAttributeType = Enum.Parse<VCardAttributeType>(row.Columns[VRaisingRelicConditionHeaderIndex.ConditionType].Value.Trim());
            _targetValue = row.Columns[VRaisingRelicConditionHeaderIndex.Value].Value.Trim();
        }

        public override bool IsTrue(VCharacter character, Dictionary<string, object> message)
        {
            switch (_cardAttributeType)
            {
                case VCardAttributeType.Type:
                    return (message["Card"] as VCard).CardType.Equals(_targetValue);
                case VCardAttributeType.Rarity:
                    return (message["Card"] as VCard).Rarity.Equals(Enum.Parse<VCardRarity>(_targetValue));
            }

            return false;
        }
    }
}