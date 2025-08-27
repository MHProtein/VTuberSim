using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.Core.Foundation;

namespace VTuber.CoopSystem.UI.DetailsUI
{
    public class VTabManager : VMonoBehaviour
    {
        [SerializeField] Color clickedColor;
        [SerializeField] List<VTabButton> tabs;
        [SerializeField] private VTabButton defaultTab;
        
        private VTabButton _currentTab;

        protected override void Awake()
        {
            base.Awake();
            foreach (var tab in tabs)
            {
                tab.Initialize(OnClick, clickedColor);
            }
        }

        protected override void Start()
        {
            base.Start();
            defaultTab.Select();
        }

        public void OnClick(VTabButton button)
        {
            if (_currentTab == button)
                return;
            if(_currentTab is not null)
                _currentTab.Unselect();
            _currentTab = button;
        }

    }
}