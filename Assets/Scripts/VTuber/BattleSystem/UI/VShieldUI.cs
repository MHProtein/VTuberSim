using TMPro;
using UnityEngine;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VShieldUI : VUIBehaviour
    {
        [SerializeField] private TMP_Text popularityText;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnShieldChange,
                dict => { popularityText.text = $"Shield: {dict["NewValue"] as int? ?? 0}"; });
        }
    }
}