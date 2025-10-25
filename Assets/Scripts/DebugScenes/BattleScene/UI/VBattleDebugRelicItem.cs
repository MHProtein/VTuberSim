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
        private VBattleDebugRelicList _relicList;

        public VRelic Relic { get; private set; }

        public bool IsSelected { get; private set; }


        public void OnPointerClick(PointerEventData eventData)
        {
            IsSelected = !IsSelected;
            if (IsSelected)
                background.color = Color.cyan;
            else
                background.color = Color.white;
        }

        public void Initialize(VRelic relic, VBattleDebugRelicList relicList)
        {
            relicName.text = relic.GetRelicName();
            relicDescription.text = relic.Description;
            Relic = relic;
        }

        public void Unselect()
        {
            IsSelected = false;
            background.color = Color.white;
        }
    }
}