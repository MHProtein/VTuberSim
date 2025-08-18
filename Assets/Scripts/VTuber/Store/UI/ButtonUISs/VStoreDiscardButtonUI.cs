using System.Collections.Generic;
using UnityEngine;
using VTuber.Core.EventCenter;
using VTuber.ScheduleSystem.UI;

namespace VTuber.Store.UI
{
    public class VStoreDiscardButtonUI : VStoreButtonUI
    {
        [SerializeField] private VCardViewSelectionUI discardCardLibraryUI;
        
        public override void OnClick()
        {
            discardCardLibraryUI.gameObject.SetActive(true);
            discardCardLibraryUI.Initialize(character.CardLibrary.GetCards(), true, false,
                confirmAction: (card) =>
                {
                    character.CardLibrary.RemoveCard(card);
                    VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnStoreEndDeleteCard,
                        new Dictionary<string, object>()
                        {
                            { "Deleted", true },
                            { "DeletedCard", card }
                        });
                    discardCardLibraryUI.Close();
                    discardCardLibraryUI.gameObject.SetActive(false);     
                    Buy();
                },
                returnAction: () =>
                {
                    VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnStoreEndDeleteCard,
                        new Dictionary<string, object>()
                        {
                            { "Deleted", false },
                        });
                    discardCardLibraryUI.Close();
                    discardCardLibraryUI.gameObject.SetActive(false);
                });
        }
    }
}