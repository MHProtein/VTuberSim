using TMPro;
using UnityEngine;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VMultiplierUI : VUIBehaviour
    {
        
        [SerializeField] private TMP_Text MultiplierText;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            VRootEventCenter.Instance.RegisterListener(VRootEventKey.OnMultiplierChange,
                dict => { MultiplierText.text = $"Multiplier: {dict["NewValue"] as int? ?? 0}%"; });
        }
    }
}