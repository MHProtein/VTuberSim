using System.Collections.Generic;
using Unity.VisualScripting;
using VTuber.BattleSystem.Core;

namespace VTuber.Core.EventCenter
{
    public enum VRaisingEventKey
    {
        Default = 0,

        OnEventStart = 1,
        OnStreamEventStart = 2,
        OnEventBeginExecute = 3,
        OnEventEnd = 4,

        OnNotifyEventDescriptionChange = 5,

        OnStaminaChanged = 6,
        OnPressureChanged = 7,
        OnSingingAbilityChanged = 8,
        OnGamingAbilityChanged = 9,
        OnChattingAbilityChanged = 10,
        OnSingingAbilityConversionRatioChanged = 11,
        OnGamingAbilityConversionRatioChanged = 12,
        OnChattingAbilityConversionRatioChanged = 13,
        OnSingingAbilityGainEfficiencyChanged = 14,
        OnGamingAbilityGainEfficiencyChanged = 15,
        OnChattingAbilityGainEfficiencyChanged = 16,
        OnFollowerCountChanged = 17,
        OnMemberCountChanged = 18,
        OnFollowerToViewerRatioChanged = 19,
        OnMoneyChanged = 20,

        OnSkipEvent = 21,
        OnSwitchToModifySchedule = 22,

        OnSelectPhaseEndingBegin = 23,
        OnPhaseEndingSelected = 24,

        OnBeginSelectCard = 25,
        OnBeginSelectCardFrom3 = 28,

        OnDayEnd = 26,
        OnWeekEnd = 36,
        OnScheduleEnd = 27,

        OnCardAdded = 29,
        OnCardRemoved = 30,
        OnCardReplaced = 31,

        OnRelicAdded = 32,
        OnRelicRemoved = 33,
        OnRelicValueChanged = 34,
        
        OnAddFollowUpEvent = 35,
        OnBeginEnding = 37,
        
        OnCooperatorAdded = 38,
        OnCooperatorRemoved = 39,
        OnCooperatorValueUpdated = 40,
        
        OnEventUISelected = 41,
        OnEventUIPlaced = 42,
        
        OnWeekStart = 43,
        OnDayStart = 44,
        
        OnSwitchToScheduleCreation = 45,
        OnSwitchToScheduleExecution = 46,
        OnFinishScheduleCreationOrModification = 47,
        
        OnSetCoopUpgradeEvent = 48,
        
        OnAddConsumable = 49,
        OnRemoveConsumable = 50,
    }
    
    public class VRaisingRootEventCenter : VEventCenter<VRaisingRootEventCenter, VRaisingEventKey, FunctionWithADict>
    {
        
    }
}