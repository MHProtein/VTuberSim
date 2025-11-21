using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.SE;
using VTuber.Dialogue.UI;
using VTuber.ScheduleSystem.Events;

namespace VTuber.EventSystem.UI
{
    public class VPhaseEndingSelectionMenu : VUIBehaviour
    {
        [SerializeField] private Button confirmButton;
        [SerializeField] private Transform grid;
        private List<VPhaseEndingOption> _options;
        private VPhaseEndingOption _selectedOption;


        public void Initialize(GameObject endingPrefab, List<VStreamEvent> endings)
        {
            Clear();
            confirmButton.interactable = false;
            _options = new List<VPhaseEndingOption>();
            foreach (var ending in endings)
            {   
                
                var endingGo = Instantiate(endingPrefab, grid);
                var option = endingGo.GetComponent<VPhaseEndingOption>();
                option.Initialize(ending, this);
                _options.Add(option);
            }
        }

        public void SelectOption(VPhaseEndingOption option)
        {
            VAudioPlayer.Instance.PlayStaticSFX(VSFXType.Selection);
            confirmButton.interactable = true;
            if (_selectedOption is not null)
                _selectedOption.Unselect();
            _selectedOption = option;
        }

        public void Confirm()
        {
            confirmButton.interactable = false;
            _selectedOption.E.Phase.SetEndingEventID(_selectedOption.E.EventID);
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnPhaseEndingSelected,
                new Dictionary<string, object>
                {
                    { "KPIs", _selectedOption.E.Kpis }
                });
            Clear();
            VEventSystemUI.Instance.ClosePhaseEndingSelectionMenu(true);
        }
        
        private void Clear()
        {
            if (_options is not null)
            {
                foreach (var option in _options) Destroy(option.gameObject);
                _options.Clear();
            }
            _selectedOption = null;
        }
    }
}