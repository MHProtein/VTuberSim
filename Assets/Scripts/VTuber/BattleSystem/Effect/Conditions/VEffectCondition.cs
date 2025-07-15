using System;
using System.Collections.Generic;
using Spire.Xls;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;
using VTuber.Core.StringToEnum;

namespace VTuber.BattleSystem.Effect.Conditions
{
    public class VConditionHeaderIndex
    {
        public const int Id = 0;
        public const int Description = 1;
        public const int Type = 2;
        public const int OperatorType = 3;
        public const int NameOrID = 4;
        public const int TargetValue = 5;
        public const int TargetDelta = 6;
    }
    
    public enum VOperatorType
    {
        LessThan,
        LessEqual,
        Equal,
        GreaterThan,
        GreaterEqual,
        NotEqual,
    }
    
    public abstract class VEffectCondition
    {
        public int id;
        VOperatorType _operatorType;
        public string description;

        public VEffectCondition(CellRange row)
        {
            id = Convert.ToInt32(row.Columns[VConditionHeaderIndex.Id].Value);
            _operatorType = Enum.Parse<VOperatorType>(row.Columns[VConditionHeaderIndex.OperatorType].Value);
            description = row.Columns[VConditionHeaderIndex.Description].Value;
        }
        
        public abstract bool IsTrue(VBattle battle, Dictionary<string, object> message);

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

        protected int ToInt(string str)
        {
            return Convert.ToInt32(str);
        }
        
    }
}