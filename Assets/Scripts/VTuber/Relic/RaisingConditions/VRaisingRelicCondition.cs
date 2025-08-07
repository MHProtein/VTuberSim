using System;
using System.Collections.Generic;
using Spire.Xls;
using VTuber.BattleSystem.Effect.Conditions;
using VTuber.Character;

namespace VTuber.Relic
{
    public static class VRaisingRelicConditionHeaderIndex
    {
        public const int Id = 0;
        public const int Name = 1;
        public const int Description = 2;
        public const int Type = 3;
        public const int Operator = 4;
        public const int ConditionType = 5;
        public const int Value = 6;
    }
    public abstract class VRaisingRelicCondition
    {
        public VOperatorType _operatorType;
        private readonly string _description;
        public uint Id => _id;
        private readonly uint _id;


        public VRaisingRelicCondition(CellRange row)
        {
            _id = Convert.ToUInt32(row.Columns[VRaisingRelicConditionHeaderIndex.Id].Value);
            _operatorType = Enum.Parse<VOperatorType>(row.Columns[VRaisingRelicConditionHeaderIndex.Operator].Value);
            _description = row.Columns[VRaisingRelicConditionHeaderIndex.Description].Value;
        }
        
        public abstract bool IsTrue(VCharacter character, Dictionary<string, object> message);
        
        protected bool Compare(int left, int right)
        {
            switch (_operatorType)
            {
                case VOperatorType.LessThan:
                    return left < right;
                case VOperatorType.LessEqual:
                    return left <= right;
                case VOperatorType.Equal:
                    return left == right;
                case VOperatorType.GreaterThan:
                    return left > right;
                case VOperatorType.GreaterEqual:
                    return left >= right;
                case VOperatorType.NotEqual:
                    return left != right;
            }

            return false;
        }
    }
}