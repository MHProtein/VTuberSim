using System.Collections.Generic;
using CsvHelper;
using UnityEditor.Sprites;
using VTuber.BattleSystem.Buff;
using VTuber.BattleSystem.Core;
using VTuber.Core.StringToEnum;

namespace VTuber.BattleSystem.Effect.Conditions
{
    public class VBuffLayerCondition : VEffectCondition
    {
        public int buffId;
        public int targetValue;

        public VBuffLayerCondition(CsvReader csv) : base(csv)
        {
            buffId = csv.GetField<int>("BuffID");
            targetValue = csv.GetField<int>("TargetValue");
        }

        public override bool IsTrue(VBattle battle, Dictionary<string, object> message)
        {
            if (battle.BuffManager.TryGetBuff(buffId, out var buff))
            {
                return Compare(buff.value, targetValue);
            }

            return false;
        }
    }
}