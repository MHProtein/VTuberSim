using TMPro;
using UnityEngine;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VPlayLeftUI : VUIBehaviour
    {
        [SerializeField] private TMP_Text PlayLeftText;

        protected override void OnEnable()
        {
            base.OnEnable();
            VRootEventCenter.Instance.RegisterListener(VRootEventKey.OnPlayLeftChange,
                dict => { PlayLeftText.text = $"PlayLeft: {dict["NewValue"] as int? ?? 0}"; });
        }
    }
}