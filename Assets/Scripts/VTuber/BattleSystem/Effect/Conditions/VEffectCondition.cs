using System;
using System.Collections.Generic;
using Spire.Xls;
using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.Core.UI;

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

    [Serializable]
    public abstract class VEffectCondition
    {
        [HideInInspector] public uint id;
        [SerializeField] protected VOperatorType operatorType;
        public string description;

        public VEffectCondition(CellRange row)
        {
            id = Convert.ToUInt32(row.Columns[VConditionHeaderIndex.Id].Value);
            operatorType = Enum.Parse<VOperatorType>(row.Columns[VConditionHeaderIndex.OperatorType].Value);
            description = row.Columns[VConditionHeaderIndex.Description].Value;
        }

        public abstract bool IsTrue(VBattle battle, Dictionary<string, object> message);

        protected int ToInt(string str)
        {
            return Convert.ToInt32(str);
        }
    }
}