using System.Collections.Generic;
using VTuber.Core.EventCenter;

namespace VTuber.BattleSystem.Core
{
    public enum VBattleEventKey
    {
        Default,
        OnBattleBegin,
        OnTurnBegin,
        OnTurnBeginBuffApply,
        OnTurnBeginLate,
        OnTurnEndBuffApply,
        OnTurnResolution,
        OnTurnEnd,
        OnBattleEnd,
        OnBattleEndNotify,
        OnBattlePause,
        
        OnDiscardToDraw,
        OnDrawCards,
        OnCardsAddedToDiscardPile,
        OnCardPlayed, //when clicked
        OnPreCardApply, //when card is about to be applied, but not yet
        OnCardBeginDisposal, //when disposal animation begins
        OnCardUsed, //when a card is played and disposed
        OnCardDisposed,
        OnTurnEndCardDisposed,
        
        OnAttributeValueChange,
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
        
        OnParameterPopularityModifierChanged,
        
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
        
        OnRelicAdded,
        OnRelicRemoved,
        OnRelicValueChanged,
    }
    
    public delegate void FunctionWithADict(Dictionary<string, object> messageDict);
    public class VBattleRootEventCenter : VEventCenter<VBattleRootEventCenter, VBattleEventKey, FunctionWithADict>
    {
        public override bool Raise(VBattleEventKey key, params object[] args)
        {
            if (key == VBattleEventKey.Default)
                return false;
            return base.Raise(key, args);
        }
    }
}