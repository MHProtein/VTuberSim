using System.Collections.Generic;
using VTuber.BattleSystem.Core;

namespace VTuber.Core.EventCenter
{
    public enum VRaisingEventKey
    {
        Default,
        
        OnEventStart,
        OnStreamEventStart,
        OnEventExecuted,
        OnScheduleExecuted,
        
        OnEventEnd,
        
        OnNotifyEventDescriptionChange,
        
        OnStaminaChanged,
        OnPressureChanged,
        OnSingingAbilityChanged,
        OnGamingAbilityChanged,
        OnChattingAbilityChanged,
        OnSingingAbilityConversionRatioChanged,
        OnGamingAbilityConversionRatioChanged,
        OnChattingAbilityConversionRatioChanged,
        OnSingingAbilityGainEfficiencyChanged,
        OnGamingAbilityGainEfficiencyChanged,
        OnChattingAbilityGainEfficiencyChanged,
        OnFollowerCountChanged,
        OnMemberCountChanged,
        OnFollowerToViewerRatioChanged,
        OnMoneyChanged,
        
        OnSkipEvent,
        OnSwitchToModifySchedule,

        OnSelectPhaseEndingBegin,
        OnPhaseEndingSelected,
        
        OnBeginSelectCard,
        
    }
    
    public class VRaisingRootEventCenter : VEventCenter<VRaisingRootEventCenter, VRaisingEventKey, FunctionWithADict>
    {
        
    }
}