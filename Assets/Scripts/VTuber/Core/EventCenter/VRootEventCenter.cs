using System.Collections.Generic;

namespace VTuber.Core.EventCenter
{
    public enum VRootEventKeys
    {
        OnTurnBegin,
        OnTurnEnd,
        OnDiscardToDraw,
        OnDrawCards,
        OnCardsAddedToDiscardPile,
        OnCardPlayed,
    }
    
    public delegate void FunctionWithADict(Dictionary<string, object> messageDict);
    public class VRootEventCenter : VEventCenter<VRootEventCenter, VRootEventKeys, FunctionWithADict>
    {
        
    }
}