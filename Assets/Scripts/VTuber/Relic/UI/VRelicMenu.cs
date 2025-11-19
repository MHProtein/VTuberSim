using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Consumable;
using VTuber.Core.Foundation;

namespace VTuber.Relic.UI
{
    public class VRelicMenu : VUIBehaviour
    {
        [SerializeField] private List<VRelicUIManager> uiManagers;
        [SerializeField] private GameObject background;
        [SerializeField] public Button showButton;
        [SerializeField] public VClickDetectionPanel detectionPanel;
        [SerializeField] private GameObject descriptionObject;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;
        public bool isShowing;

        protected override void Awake()
        {
            base.Awake();
            showButton.onClick.AddListener(Show);
            detectionPanel.onClick += Show;
        }

        public void Show()
        {
            isShowing = !isShowing;
            detectionPanel.gameObject.SetActive(isShowing);
            background.gameObject.SetActive(isShowing);
            foreach (var uiManager in uiManagers) uiManager.Show(isShowing);
        }

        public void SetDescription(VRelic relic)
        {
            if(relic is null)
            {
                descriptionObject.SetActive(false);
                return;
            }
            descriptionObject.SetActive(true);
            descriptionText.text = relic.Description;
            nameText.text = relic.GetRelicName();
        }
    }
}