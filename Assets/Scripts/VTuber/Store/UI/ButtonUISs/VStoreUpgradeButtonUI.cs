using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VTuber.Core.EventCenter;
using VTuber.ScheduleSystem.UI;

namespace VTuber.Store.UI
{
    public class VStoreUpgradeButtonUI : VStoreButtonUI
    {
        [SerializeField] private VCardViewSelectionUI upgradeCardLibraryUI;

        public override void OnClick()
        {
            base.OnClick();
            upgradeCardLibraryUI.gameObject.SetActive(true);
            upgradeCardLibraryUI.Initialize(character.CardLibrary.GetCards().Where(card => !card.IsUpgraded).ToList(),
                true, true, false,
                card =>
                {
                    card.Upgrade(false);
                    VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnStoreEndUpgradeCard,
                        new Dictionary<string, object>
                        {
                            { "Upgraded", true },
                            { "UpgradedCard", card }
                        });
                    upgradeCardLibraryUI.Close();
                    upgradeCardLibraryUI.gameObject.SetActive(false);
                    Buy();
                },
                () =>
                {
                    VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnStoreEndUpgradeCard,
                        new Dictionary<string, object>
                        {
                            { "Upgraded", false }
                        });
                    upgradeCardLibraryUI.Close();
                    upgradeCardLibraryUI.gameObject.SetActive(false);
                },
                card => card.Upgrade(false));
        }
    }
}