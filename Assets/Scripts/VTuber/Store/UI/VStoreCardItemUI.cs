using UnityEngine;
using VTuber.BattleSystem.UI;
using VTuber.Character;

namespace VTuber.Store.UI
{
    public class VStoreCardItemUI : VStoreItemUI
    {
        [SerializeField] private VCardUI cardUI;

        public override void SetSlot(VStoreSlot slot, VCharacter character)
        {
            cardUI.SetCard((slot as VStoreCardSlot).card);
            base.SetSlot(slot, character);
        }
    }
}