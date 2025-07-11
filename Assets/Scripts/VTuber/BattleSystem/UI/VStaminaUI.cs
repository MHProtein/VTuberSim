using TMPro;
using UnityEngine;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VStaminaUI : VUIBehaviour
    {
        [SerializeField] private TMP_Text popularityText;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnStaminaChange,
                dict => { popularityText.text = $"Stamina: {dict["NewValue"] as int? ?? 0}"; });
        }
    }
}