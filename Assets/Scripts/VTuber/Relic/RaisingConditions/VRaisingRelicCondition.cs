using System;
using System.Collections.Generic;
using Spire.Xls;
using VTuber.Character;
using VTuber.Core.UI;

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
        private readonly string _description;
        public VOperatorType operatorType;


        public VRaisingRelicCondition(CellRange row)
        {
            Id = Convert.ToUInt32(row.Columns[VRaisingRelicConditionHeaderIndex.Id].Value);
            operatorType = Enum.Parse<VOperatorType>(row.Columns[VRaisingRelicConditionHeaderIndex.Operator].Value);
            _description = row.Columns[VRaisingRelicConditionHeaderIndex.Description].Value;
        }

        public uint Id { get; }

        public abstract bool IsTrue(VCharacter character, Dictionary<string, object> message);
    }
}