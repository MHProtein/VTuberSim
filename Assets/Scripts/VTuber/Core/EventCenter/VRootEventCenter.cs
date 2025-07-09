using System.Collections.Generic;

namespace VTuber.Core.EventCenter
{
    public enum VRootEventKey
    {
        Default,
        OnTurnBegin,
        OnTurnEnd,
        OnDiscardToDraw,
        OnDrawCards,
        OnCardsAddedToDiscardPile,
        OnCardPlayed,
        OnTurnResolution,
        OnPlayLeftChange,
        OnTurnChange,
        OnParameterChange,
        OnMultiplierChange,
        OnPopularityChange,
    }
    
    public delegate void FunctionWithADict(Dictionary<string, object> messageDict);
    public class VRootEventCenter : VEventCenter<VRootEventCenter, VRootEventKey, FunctionWithADict>
    {
        
    }
}