using System.Collections.Generic;
using VTuber.BattleSystem.Core;

namespace VTuber.Core.EventCenter
{
    public enum VRaisingEventKey
    {
        Default,
        
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
    }
    
    public class VRaisingRootEventCenter : VEventCenter<VRaisingRootEventCenter, VRaisingEventKey, FunctionWithADict>
    {
        
    }
}