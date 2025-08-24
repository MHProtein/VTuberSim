using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Core.UI.VAccountSelection
{
    public class VAccountSlot : VUIBehaviour
    {
        public VAccountUI Account => _accountUI;
        private VAccountUI _accountUI;
        

        public void SetAccountUI(VAccountUI accountUI)
        {
            _accountUI = accountUI;
        }

        public void RemoveAccountUI()
        {
            _accountUI = null;
        }

        public bool HasAccountUI() => _accountUI is not null;
    }
}