using System.Collections.Generic;

namespace VTuber.Core.EventCenter
{
    public enum VRootEventKey
    {
        Default,
        OnBattleBegin,
        OnTurnBegin,
        OnTurnEndBuffApply,
        OnTurnResolution,
        OnTurnEnd,
        
        OnDiscardToDraw,
        OnDrawCards,
        OnCardsAddedToDiscardPile,
        OnCardPlayed,
        OnPreCardApply,
        OnCardDisposed,
        OnTurnEndCardDisposed,
        
        OnPlayLeftChange,
        OnTurnChange,
        OnParameterChange,
        OnMultiplierChange,
        OnPopularityChange,
        OnStaminaChange,
        OnShieldChange,
        
        OnBuffAdded,
        OnBuffRemoved,
        OnBuffValueUpdated,
        
        OnNotifyTurnBeginDelay,
        
        OnRequestDrawCards,
        OnRedrawCards
    }
    
    public delegate void FunctionWithADict(Dictionary<string, object> messageDict);
    public class VBattleRootEventCenter : VEventCenter<VBattleRootEventCenter, VRootEventKey, FunctionWithADict>
    {
        
    }
}