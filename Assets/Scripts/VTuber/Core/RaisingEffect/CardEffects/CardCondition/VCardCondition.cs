using System;
using Spire.Xls;
using VTuber.BattleSystem.Card;

namespace VTuber.Core.RaisingEffect
{
    public class VCardConditionHeaderIndex
    {
        public const int Id = 0;
        public const int Type = 3;
        public const int Condition = 4;
    }

    public abstract class VCardCondition
    {
        public VCardCondition(CellRange row)
        {
            ID = Convert.ToUInt32(row.Columns[VCardConditionHeaderIndex.Id].Value);
        }

        public uint ID { get; }

        public abstract bool IsTrue(VCard card);
        public abstract bool IsTrue(VCardConfiguration cardConfig);
    }
}