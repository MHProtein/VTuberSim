using System.Collections.Generic;

namespace VTuber.Core.EventCenter
{
    public enum VRootEventKey
    {
        Default,
        OnTurnBegin,
        OnTurnEndBuffApply,
        OnTurnResolution,
        OnTurnEnd,
        
        OnDiscardToDraw,
        OnDrawCards,
        OnCardsAddedToDiscardPile,
        OnCardPlayed,
        
        OnPlayLeftChange,
        OnTurnChange,
        OnParameterChange,
        OnMultiplierChange,
        OnPopularityChange,
        OnStaminaChange,
        
        OnBuffAdded,
        OnBuffRemoved,
        OnBuffValueUpdated,
    }
    
    public delegate void FunctionWithADict(Dictionary<string, object> messageDict);
    public class VRootEventCenter : VEventCenter<VRootEventCenter, VRootEventKey, FunctionWithADict>
    {
        
    }
}