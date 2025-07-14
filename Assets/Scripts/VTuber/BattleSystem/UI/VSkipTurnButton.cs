using UnityEngine.UI;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VSkipTurnButton : VUIBehaviour
    {
        private Button button;

        protected override void Awake()
        {
            base.Awake();
            button = GetComponent<Button>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnTurnBegin,
                dict => button.interactable = true);
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnTurnEnd,
                dict => button.interactable = false);
        }
    }
}