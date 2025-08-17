using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.ScheduleSystem.UI;

namespace VTuber.Store.UI
{
    public class VStoreUI : VUIBehaviour
    {
        [SerializeField] private Button UpgradeCardButton;
        [SerializeField] private Button DiscardCardButton;
        [FormerlySerializedAs("deleteCardLibraryUI")] [SerializeField] private VCardViewSelectionUI discardCardLibraryUI;
        [SerializeField] private VCardViewSelectionUI upgradeCardLibraryUI;
        VCharacter _character;

        protected override void Awake()
        {
            base.Awake();
            //UpgradeCardButton.onClick.AddListener(() => upgradeCardLibraryUI.Initialize(_character.CardLibrary.GetCards(), false));
            DiscardCardButton.onClick.AddListener(OnStoreBeginDeleteCard);
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEnterStore, OnEnterStore);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEnterStore, OnEnterStore);
        }
        
        private void OnEnterStore(Dictionary<string, object> messagedict)
        {
            _character = messagedict["Character"] as VCharacter;
        }
        
        private void OnStoreBeginDeleteCard()
        {
            discardCardLibraryUI.gameObject.SetActive(true);
            discardCardLibraryUI.Initialize(_character.CardLibrary.GetCards(), true, false,
            confirmAction: (card) =>
            {
                _character.CardLibrary.RemoveCard(card);
                VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnStoreEndDeleteCard,
                    new Dictionary<string, object>()
                    {
                        { "Deleted", true },
                        { "DeletedCard", card }
                    });
                discardCardLibraryUI.Close();
                discardCardLibraryUI.gameObject.SetActive(false);
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

        public void Initialze(VCharacter character)
        {
            _character = character;
        }
    }
}