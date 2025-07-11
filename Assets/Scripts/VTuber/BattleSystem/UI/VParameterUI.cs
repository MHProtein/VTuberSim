using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VParameterUI : VUIBehaviour
    {
        [SerializeField] private TMP_Text ParameterText;

        protected override void OnEnable()
        {
            base.OnEnable();
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnParameterChange,
                dict => { ParameterText.text = $"Parameter: {dict["NewValue"] as int? ?? 0}"; });
        }
    }
}