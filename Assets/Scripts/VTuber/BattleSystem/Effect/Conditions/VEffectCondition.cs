using System;
using System.Collections.Generic;
using CsvHelper;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;
using VTuber.Core.StringToEnum;

namespace VTuber.BattleSystem.Effect.Conditions
{
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
        public readonly int id;
        VOperatorType _operatorType;
        public string description;

        public VEffectCondition(CsvReader csv)
        {
            id = csv.GetField<int>("ID");
            _operatorType = Enum.Parse<VOperatorType>(csv.GetField<string>("OperatorType"));
            description = csv.GetField<string>("Description");
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
    }
}