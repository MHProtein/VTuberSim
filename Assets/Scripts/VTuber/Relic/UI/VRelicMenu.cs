using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VTuber.Consumable;
using VTuber.Core.Foundation;

namespace VTuber.Relic.UI
{
    public class VRelicMenu : VUIBehaviour
    {
        [SerializeField] List<VRelicUIManager> uiManagers;
        [SerializeField] GameObject background;
        [SerializeField] public Button showButton;
        [SerializeField] public VClickDetectionPanel detectionPanel;
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
            foreach (var uiManager in uiManagers)
            {
                uiManager.Show(isShowing);
            }
        }
    }
}