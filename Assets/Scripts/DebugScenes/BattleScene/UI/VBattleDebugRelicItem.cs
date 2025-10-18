using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VTuber.Core.Foundation;
using VTuber.Relic;

namespace DebugScenes.BattleScene.UI
{
    public class VBattleDebugRelicItem : VUIBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMP_Text relicName;
        [SerializeField] private TMP_Text relicDescription;
        [SerializeField] private Image background;
        
        public VRelic Relic => _relic;
        private VRelic _relic;
        private VBattleDebugRelicList _relicList;
        
        public bool IsSelected => _isSelected;
        private bool _isSelected;
        public void Initialize(VRelic relic, VBattleDebugRelicList relicList)
        {
            relicName.text = relic.GetRelicName();
            relicDescription.text = relic.Description;
            _relic = relic;
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            _isSelected = !_isSelected;
            if(_isSelected)
                background.color = Color.cyan;
            else
                background.color = Color.white;
        }

        public void Unselect()
        {
            _isSelected = false;
            background.color = Color.white;
        }
    }
}