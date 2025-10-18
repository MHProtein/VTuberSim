using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.BattleAttribute
{
    public class VBattleTurnAttribute : VBattleAttribute
    {
        public int MaxTurn { get; private set; }
        public int TurnIndex => MaxTurn - Value;
        public VBattleTurnAttribute(int maxTurn) : base(maxTurn, false, VBattleEventKey.OnTurnChange)
        {
            MaxTurn = maxTurn;
        }
        
        public VBattleTurnAttribute(VBattleAttributeSaveData saveData) : base(saveData)
        {
            MaxTurn = saveData.maxTurn;
        }

        public override VBattleAttributeSaveData Save()
        {
            var data = base.Save();
            data.maxTurn = MaxTurn;
            return data;
        }

        protected override void InitSetValue(int value, bool isFromCard, bool shouldPlayTwice = false)
        {
            Value = Mathf.Clamp(value, _minValue, _maxValue);
            HighestValue = Value;
            SendEvent(Value, 0, isFromCard, shouldPlayTwice);
        }
    }
}