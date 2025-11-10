using System;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace VTuber.ScheduleSystem.UI.RaisingAnimationSystem
{
    public class VEffectCardsAnimation : VRaisingAnimation
    {
        [SerializeField] private GameObject effectCardPrefab;
        [SerializeField] private Transform effectCardsContent;
        [SerializeField] private Scrollbar scrollbar;
        
        private List<VEffectCard> _effectCards = new();
        
        public override void BeginAnimation(VAnimationRequest request, Action onCompleted)
        {
            base.BeginAnimation(request, onCompleted);
            
            var effectCardObject = Instantiate(effectCardPrefab, effectCardsContent);
            var effectCard = effectCardObject.GetComponent<VEffectCard>();
            effectCard.SetEffect(request);
            _effectCards.Add(effectCard);

            Tween.Custom(scrollbar.value, 0.0f, 0.5f, value => scrollbar.value = value).OnComplete(() =>
            {
                onCompleted?.Invoke();
            });
        }
    }
}