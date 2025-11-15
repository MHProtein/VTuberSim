using System;
using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Core;
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
        public VAnimationType animationType;
        public Action effectApply;
        public VCooperator coop;
        public uint relicId;
        public List<uint> consumableIDs;
        public bool returnable; //for consumable menu, determines if return button shows
        public List<VCard> cards;
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
    }
}