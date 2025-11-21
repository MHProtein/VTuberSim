using System;
using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.UI;
using VTuber.Consumable;
using VTuber.CoopSystem;
using VTuber.Core.RaisingEffect;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.RaisingAnimationSystem
{
    public class VAnimationRequest
    {
        public VInstigatorType instigatorType;
        public Sprite instigatorIcon;
        public Sprite attributeIcon;
        public string description;
        public int value;
        public bool isMaxValue;
        public bool isPercentage;
        public VAnimationType animationType;
        public Action effectApply;
        public VCooperator coop;
        public uint relicId;
        public List<uint> consumableIDs;
        public bool returnable; //for consumable/card menu, determines if return button shows
        public List<VCard> cards;
        public bool cardSelectable;
        public VAnimationType cardSelectAnimationType;
        public Action<VCard> cardSelectConfirmAction;
        public Action<VCard> cardSelectPreviewAction;
        public Action cardSelectReturnAction;
        public VCard previewCard;
    }

    public class VAnimationRequestFactory
    {
        public static VAnimationRequest Create(VInstigatorType instigatorType, Sprite icon, string description)
        {
            return new VAnimationRequest
            {
                instigatorType = instigatorType,
                instigatorIcon = icon,
                description = description,
            };
        }

        public static VAnimationRequest CreateAddConsumableRequest(VConsumable consumable, bool returnable)
        {
            return new VAnimationRequest()
            {
                instigatorType = VInstigatorType.Ignore,
                animationType = VAnimationType.AddConsumable,
                consumableIDs = new List<uint>(){ consumable.ConfigId },
                effectApply = () => VGameManager.Instance.Character.ConsumableManager.AddConsumable(consumable),
                returnable = returnable
            };
        }

        public static VAnimationRequest CreateAddCardRequest(VCard card)
        {            
            return new VAnimationRequest()
            {
                instigatorType = VInstigatorType.Ignore,
                animationType = VAnimationType.AddCard,
                cards = new (){ card },
                effectApply = () => VGameManager.Instance.Character.CardLibrary.AddCard(card),
            };
        }

        public static VAnimationRequest CreateRemoveCardRequest(VCard card)
        {
            return new VAnimationRequest()
            {
                instigatorType = VInstigatorType.Ignore,
                animationType = VAnimationType.RemoveCard,
                cards = new (){ card },
                effectApply = () => VGameManager.Instance.Character.CardLibrary.RemoveCard(card),
            };
        }
        
        public static VAnimationRequest CreateUpgradeCardRequest(VCard card)
        {
            return new VAnimationRequest()
            {
                instigatorType = VInstigatorType.Ignore,
                animationType = VAnimationType.UpgradeCard,
                cards = new (){ card },
            };
        }
        
        public static VAnimationRequest CreateReplaceCardRequest(VCard cardToReplace, VCard cardToBeReplaced)
        {
            return new VAnimationRequest()
            {
                instigatorType = VInstigatorType.Ignore,
                animationType = VAnimationType.ReplaceCard,
                cards = new (){ cardToReplace, cardToBeReplaced },
            };
        }
        
        public static VAnimationRequest CreateSelectCardRequest(List<VCard> cards, 
            bool returnable, bool cardSelectable, VAnimationType cardSelectAnimationType,
            Action<VCard> cardSelectConfirmAction, Action returnAction = null, Action<VCard> previewAction = null)
        {
            return new VAnimationRequest()
            {
                instigatorType = VInstigatorType.Ignore,
                animationType = previewAction == null ? VAnimationType.SelectCard : VAnimationType.SelectCardPreview,
                cardSelectPreviewAction = previewAction,
                cardSelectReturnAction = returnAction,
                returnable = returnable,
                cardSelectable = cardSelectable,
                cards = cards,
                cardSelectAnimationType = cardSelectAnimationType,
                cardSelectConfirmAction = cardSelectConfirmAction,
            };
        }


    }
}