using System;
using System.Linq;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VTuber.ScheduleSystem.UI.RaisingAnimationSystem
{
    public class VAttributeAnimation : VRaisingAnimation
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text valueText;
        
        public override void BeginAnimation(VAnimationRequest request, Action onComplete)
        {
            base.BeginAnimation(request, onComplete);
            icon.sprite = request.instigatorIcon;
            string valueTextStr = "";
            if (request.value > 0)
            {
                valueTextStr += "+";
                valueText.color = Color.green;
            }
            else
                valueText.color = Color.red;
            valueTextStr += request.value;
            valueText.text = valueTextStr;
            ShowHideUI(true);
            Tween.Delay(this, 0.5f, () =>
            {
                ShowHideUI(false);
                onComplete?.Invoke();
            });
        }
        
        private void ShowHideUI(bool show)
        {
            icon.gameObject.SetActive(show);
            valueText.gameObject.SetActive(show);
        }
    }
}