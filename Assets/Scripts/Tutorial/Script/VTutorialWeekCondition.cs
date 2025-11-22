using System;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.UI;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Events;
using VTuber.ScheduleSystem.UI;

namespace Tutorial.Script
{
    public enum VTutorialWeekConditionType
    {
        Attribute,
        CardLibrarySize,
        CoopValue,
        CoopLevel,
        WeeklyEventKPI
    }

    [Serializable]
    public class VTutorialWeekCondition
    {
        public VTutorialWeekConditionType conditionType;
        public VEventType eventType;
        public string idOrName;
        public VOperatorType operatorType;
        public int targetValue;

        public string GetConditionDescription(VCharacter character)
        {
            switch (conditionType)
            {
                case VTutorialWeekConditionType.Attribute:
                {
                    return $"{VUIUtils.GetAttributeName(idOrName)} {VUIUtils.GetOperatorTypeName(operatorType)} {targetValue}";
                }
                case VTutorialWeekConditionType.CardLibrarySize:
                {
                    return $"卡牌库大小 {VUIUtils.GetOperatorTypeName(operatorType)} {targetValue}";
                }
                case VTutorialWeekConditionType.CoopValue:
                {
                    return $"{character.CooperatorManager.GetCooperator(uint.Parse(idOrName)).CoopName}好感度 {VUIUtils.GetOperatorTypeName(operatorType)} {targetValue}";
                }
                case VTutorialWeekConditionType.CoopLevel:
                {
                    var coop = character.CooperatorManager.GetCooperator(uint.Parse(idOrName));
                    return $"{coop.CoopName}等级 {VUIUtils.GetOperatorTypeName(operatorType)} {coop.GetLevelName(targetValue)}";
                }
                case VTutorialWeekConditionType.WeeklyEventKPI:
                {
                    if (idOrName.IsNullOrWhitespace())
                    {
                        return $"安排{VUIUtils.Instance.GetEventName(eventType)}类型事件 {VUIUtils.GetOperatorTypeName(operatorType)} {targetValue}";
                    }
                    else
                    {
                        var id = uint.Parse(idOrName);
                        VScheduleEventConfiguration e;
                        if (eventType == VEventType.Stream)
                        {
                            e = VDataManager.Instance.GetStreamEventConfigurationByID(id);
                        }
                        else
                            e = VDataManager.Instance.GetDialogueEventConfigurationByID(id);
                        return $"安排{e.eventName}事件 {VUIUtils.GetOperatorTypeName(operatorType)} {targetValue}";
                    }
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public bool IsTrue(VCharacter character)
        {
            switch (conditionType)
            {
                case VTutorialWeekConditionType.Attribute:
                {
                    if (character.AttributeManager.TryGetAttribute(idOrName, out var attribute))
                        return VMathUtils.Compare(attribute.Value, targetValue, operatorType);
                    return false;
                }
                case VTutorialWeekConditionType.CardLibrarySize:
                {
                    return VMathUtils.Compare(character.CardLibrary.GetCardLibrarySize(), targetValue, operatorType);
                }
                case VTutorialWeekConditionType.CoopValue:
                {
                    return VMathUtils.Compare(character.CooperatorManager.GetCooperator(uint.Parse(idOrName)).CoopValue,
                        targetValue, operatorType);
                }
                case VTutorialWeekConditionType.CoopLevel:
                {
                    return VMathUtils.Compare(character.CooperatorManager.GetCooperator(uint.Parse(idOrName)).CurrentCoopLevel.levelIndex,
                        targetValue, operatorType);
                }
                case VTutorialWeekConditionType.WeeklyEventKPI:
                {
                    if (idOrName.IsNullOrWhitespace())
                    {
                        return VMathUtils.Compare(character.eventsCompleted[eventType].Count, targetValue, operatorType);
                    }
                    else
                    {
                        if (!uint.TryParse(idOrName, out var id))
                        {
                            VDebug.LogError($"WeeklyEventKPI condition idOrName {idOrName} is not a valid uint.");
                            return false;
                        }
                        return VMathUtils.Compare(character.eventsCompleted[eventType].Count(e => e == id), targetValue, operatorType);
                    }
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}