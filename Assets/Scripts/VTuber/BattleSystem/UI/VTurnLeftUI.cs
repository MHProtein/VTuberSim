using TMPro;
using UnityEngine;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VTurnLeftUI : VUIBehaviour
    {
        [SerializeField] private TMP_Text turnLeftText;

        protected override void OnEnable()
        {
            base.OnEnable();
  
            VRootEventCenter.Instance.RegisterListener(VRootEventKey.OnTurnChange, dict =>
            {
                turnLeftText.text = $"TurnLeft: {dict["NewValue"] as int? ?? 0}";
            });
        }
    }
}