using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using VTuber.Character;
using VTuber.Core.ScriptSystem;

namespace Tutorial.Script
{
    [Serializable]
    public class VTipConfig
    {
        public string title;
        [TextArea]public string description;
        public Sprite image;
    }
    [Serializable]
    public class VTutorialWeek
    {
        public int weekID;
        [LabelText("产生帮助事件")] public bool useCoopEvents;
        [LabelText("每周可使用事件")] public List<uint> eventIDs;
        [LabelText("每周可使用直播事件")] public List<uint> streamEventIDs;
        [LabelText("周结束条件")] public List<VTutorialWeekCondition> conditions;
        [LabelText("周提示")] public VTipConfig tip;

        public bool IsAllConditionsTrue(VCharacter character)
        {
            foreach (var condition in conditions)
                if (!condition.IsTrue(character))
                    return false;
            return true;
        }
    }

    public class VTutorialScriptConfiguration : VScriptConfiguration
    {
        [Space(5)] [Header("教程周")] public List<VTutorialWeek> weeks;
    }
}