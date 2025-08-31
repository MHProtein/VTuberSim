using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using VTuber.Consumable;

namespace VTuber.CoopSystem.UI.DetailsUI
{
    public class VCoopDetails : UIBehaviour
    {
        [SerializeField] private GameObject ui;
        [SerializeField] private VClickDetectionPanel detectionPanel;
        [SerializeField] private TMP_Text coopName;
        [SerializeField] private TMP_Text coopValue;
        [SerializeField] private List<VCoopTab> tabs;

        public Action onHide;

        protected override void Awake()
        {
            base.Awake();
            detectionPanel.onClick += Hide;
        }

        public void SetCooperator(VCooperator cooperator)
        {
            foreach (var tab in tabs)
            {
                tab.Clear();
            }
            coopName.text = cooperator.configuration.Name;
            var nextLevelIndex = cooperator.CurrentLevel + 1;
            if (nextLevelIndex >= cooperator.configuration.CoopLevels.Count)
            {
                coopValue.text = "好感度已满";
            }
            else
            {
                coopValue.text = $"{cooperator.CoopValue}/{cooperator.configuration.CoopLevels[nextLevelIndex].from}";
            }

            foreach (var tab in tabs)
            {
                tab.SetTab(cooperator);
            }
        }

        public void Show()
        {
            ui.gameObject.SetActive(true);
        }

        public void Hide()
        {
            ui.gameObject.SetActive(false);
            foreach (var tab in tabs)
            {
                tab.Clear();
            }
            onHide?.Invoke();          
        }
    }
}