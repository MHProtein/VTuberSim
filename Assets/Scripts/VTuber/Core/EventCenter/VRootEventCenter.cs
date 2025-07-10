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
        OnCardDisposed,
        
        OnPlayLeftChange,
        OnTurnChange,
        OnParameterChange,
        OnMultiplierChange,
        OnPopularityChange,
        OnStaminaChange,
        
        OnBuffAdded,
        OnBuffRemoved,
        OnBuffValueUpdated,
        
        OnNotifyTurnBeginDelay,
        
        OnRequestDrawCards,
        OnRedrawCards
    }
    
    public delegate void FunctionWithADict(Dictionary<string, object> messageDict);
    public class VRootEventCenter : VEventCenter<VRootEventCenter, VRootEventKey, FunctionWithADict>
    {
        
    }
}