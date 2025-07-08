using System.Collections.Generic;

namespace VTuber.Core.EventCenter
{
    public static class VRootEventKeys
    {
        public const string OnTurnBegin = "OnTurnBegin";
        public const string OnTurnEnd = "OnTurnEnd";
        public const string OnDiscardToDraw = "OnDiscardToDraw";
        public const string OnDrawCards = "OnDrawCards";
        public const string OnCardsAddedToDiscardPile = "OnCardsAddedToDiscardPile";
        
        public const string OnCardPlayed = "OnCardPlayed";
        // public const string OnCardExhausted = "OnCardExhausted";
        // public const string OnCardDiscarded = "OnCardDiscarded";
    }
    
    public delegate void FunctionWithADict(Dictionary<string, object> messageDict);
    public class VRootEventCenter : VEventCenter<VRootEventCenter, string, FunctionWithADict>
    {
        
    }
}