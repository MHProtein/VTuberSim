using System.Collections.Generic;
using NUnit.Framework;
using Sirenix.OdinInspector;
using UnityEngine;
using VTuber.BattleSystem.Effect.Conditions;
using VTuber.Core.Foundation;

namespace Tutorial.Script
{
    public class VTutorialStreamEventConfiguration : VScriptableObject
    {
        [LabelText("基础直播事件ID")] public uint baseEventID;
        [LabelText("直播通过条件")] public List<VAttributeCondition> conditions;
        [LabelText("直播可用牌")] public List<uint> deck;
        [LabelText("每回合手牌")] public Dictionary<int, List<uint>> turnHandCards;
        [LabelText("直播提示")] public List<Sprite> tips;
    }
}