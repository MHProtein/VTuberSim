using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Core.Foundation;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.RaisingAnimationSystem.Animations.EffectCardsAnimation
{
    public class VEffectCard : VUIBehaviour
    {
        public bool isAvailable = true;
        public int index;
        [SerializeField] private TMP_Text effectDescriptionText;
        [SerializeField] private Image instigatorIcon;
        [SerializeField] private Image attributeIcon;
        
        public void SetEffect(VAnimationRequest request, bool debug)
        {
            effectDescriptionText.text = request.description;
            if(!debug)
                instigatorIcon.sprite = request.instigatorIcon;
            attributeIcon.sprite = request.attributeIcon;
        }
    }
}