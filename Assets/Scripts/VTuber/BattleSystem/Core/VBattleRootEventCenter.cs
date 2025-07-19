using System.Collections.Generic;
using VTuber.Core.EventCenter;

namespace VTuber.BattleSystem.Core
{
    public enum VBattleEventKey
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
        OnCardPlayed, //when clicked
        OnPreCardApply, //when card is about to be applied, but not yet
        OnCardBeginDisposal, //when disposal animation begins
        OnCardUsed, //when a card is played and disposed
        OnCardDisposed,
        OnTurnEndCardDisposed,
        
        OnPlayLeftChange,
        OnTurnChange,
        OnParameterChange,
        OnMultiplierChange,
        OnPopularityChange,
        OnStaminaChange,
        OnShieldChange,
        OnMembershipCountChange,
        OnViewerCountChange,
        OnRevenueChange,
        
        OnBuffAdded,
        OnBuffRemoved,
        OnBuffValueUpdated,
        
        OnNotifyTurnBeginDelay,
        
        OnRequestDrawCards,
        OnRedrawCards,
        
        OnCardMovedToHandSlot,
        OnCardMovedToPlayPosition,
        OnNotifyBeginDisposeCard,
        OnPlayTheSecondTime,
        OnSkipTurnClicked,
        
        OnRequestPickCardsFromPile,
        OnBeginPickCardsFromPile,
        OnCardsPickedFromPile,
        
        OnMultiplierSequenceCalculated,
        
    }
    
    public delegate void FunctionWithADict(Dictionary<string, object> messageDict);
    public class VBattleRootEventCenter : VEventCenter<VBattleRootEventCenter, VBattleEventKey, FunctionWithADict>
    {
        
    }
}