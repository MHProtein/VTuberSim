using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Core.UI.VAccountSelection
{
    public class VAccountSlot : VUIBehaviour
    {
        public VAccountUI Account { get; private set; }


        public void SetAccountUI(VAccountUI accountUI)
        {
            Account = accountUI;
        }

        public void RemoveAccountUI()
        {
            Account = null;
        }

        public bool HasAccountUI()
        {
            return Account is not null;
        }
    }
}