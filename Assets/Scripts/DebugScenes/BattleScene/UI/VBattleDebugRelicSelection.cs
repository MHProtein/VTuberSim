using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Relic;

namespace DebugScenes.BattleScene.UI
{
    public class VBattleDebugRelicSelection : VUIBehaviour
    {
        [SerializeField] private VBattleDebugRelicList relicList;
        [SerializeField] private VBattleDebugRelicList selectedRelicList;
        [SerializeField] private Button toRelicContent;
        [SerializeField] private Button toSelectedRelicContent;
        [SerializeField] private Button clearSelectRelicButton;

        public void Initialize()
        {
            toRelicContent.onClick.AddListener(OnToRelicContentClick);
            toSelectedRelicContent.onClick.AddListener(OnToSelectedRelicContentClick);
            clearSelectRelicButton.onClick.AddListener(ClearSelectedRelic);
            
            relicList.Initialize(VDataManager.Instance.Relics.Select(config => config.Value.CreateRelic()));
            selectedRelicList.Initialize(null);
        }

        private void ClearSelectedRelic()
        {
            relicList.AddItems(selectedRelicList.GetAll());
        }

        private void OnToSelectedRelicContentClick()
        {
            selectedRelicList.AddItems(relicList.GetSelected());
        }

        private void OnToRelicContentClick()
        {
            relicList.AddItems(selectedRelicList.GetSelected());
        }

        public List<VBattleRelic> GetSelected()
        {
            return selectedRelicList.GetRelics();
        }
    }
}