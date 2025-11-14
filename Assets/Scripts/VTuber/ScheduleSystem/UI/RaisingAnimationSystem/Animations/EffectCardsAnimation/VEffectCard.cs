using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VTuber.Core.Foundation;
using VTuber.Core.RaisingEffect;

namespace VTuber.ScheduleSystem.UI.RaisingAnimationSystem
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