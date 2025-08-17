using System.Collections.Generic;
using UnityEngine;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.ScheduleSystem.UI;

namespace VTuber.Store.UI
{
    public class VStoreUI : VUIBehaviour
    {
        [SerializeField] private VCardViewSelectionUI deleteCardLibraryUI;
        [SerializeField] private VCardViewSelectionUI upgradeCardLibraryUI;
        VCharacter _character;

        protected override void OnEnable()
        {
            base.OnEnable();
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEnterStore, OnEnterStore);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnStoreBeginDeleteCard, OnStoreBeginDeleteCard);
        }



        protected override void OnDisable()
        {
            base.OnDisable();
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEnterStore, OnEnterStore);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnStoreBeginDeleteCard, OnStoreBeginDeleteCard);
        }
        
        private void OnEnterStore(Dictionary<string, object> messagedict)
        {
            _character = messagedict["Character"] as VCharacter;
        }
        
        private void OnStoreBeginDeleteCard(Dictionary<string, object> messagedict)
        {
            deleteCardLibraryUI.Initialize(_character.CardLibrary.GetCards(), true, 
            confirmAction: (card) =>
            {
                _character.CardLibrary.RemoveCard(card);
                VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnStoreEndDeleteCard,
                    new Dictionary<string, object>()
                    {
                        { "Deleted", true },
                        { "DeletedCard", card }
                    });
                deleteCardLibraryUI.Close();
                deleteCardLibraryUI.gameObject.SetActive(false);
            },
            returnAction: () =>
            {
                VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnStoreEndDeleteCard,
                    new Dictionary<string, object>()
                    {
                        { "Deleted", false },
                    });
                deleteCardLibraryUI.Close();
                deleteCardLibraryUI.gameObject.SetActive(false);
            });
            deleteCardLibraryUI.gameObject.SetActive(true);
        }
    }
}