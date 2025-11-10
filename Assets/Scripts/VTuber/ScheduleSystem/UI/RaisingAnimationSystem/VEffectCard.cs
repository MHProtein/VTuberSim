using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Core.Foundation;
using VTuber.Core.RaisingEffect;

namespace VTuber.ScheduleSystem.UI.RaisingAnimationSystem
{
    public class VEffectCard : VUIBehaviour
    {
        [SerializeField] private TMP_Text effectDescriptionText;
        [SerializeField] private Image effectIcon;
        
        public void SetEffect(VAnimationRequest request)
        {
            effectDescriptionText.text = request.description;
            effectIcon.sprite = request.instigatorIcon;
        }
    }
}