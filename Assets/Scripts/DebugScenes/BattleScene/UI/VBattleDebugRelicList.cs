using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VTuber.Core.Foundation;
using VTuber.Relic;

namespace DebugScenes.BattleScene.UI
{
    public class VBattleDebugRelicList : VUIBehaviour
    {
        [SerializeField] private Transform content;
        [SerializeField] private GameObject relicItemPrefab;
        private List<VBattleDebugRelicItem> _relicItems;

        public void Initialize(IEnumerable<VRelic> relics)
        {
            _relicItems = new List<VBattleDebugRelicItem>();
            if (relics is not null)
                _relicItems.AddRange(relics.Where(relic => relic is VBattleRelic).Select(relic =>
                {
                    var relicItem = Instantiate(relicItemPrefab, content).GetComponent<VBattleDebugRelicItem>();
                    relicItem.Initialize(relic, this);
                    return relicItem;
                }));
        }

        public IEnumerable<VBattleDebugRelicItem> GetSelected()
        {
            return _relicItems.Where(item => item.IsSelected).Where(item => item.IsSelected);
        }

        public IEnumerable<VBattleDebugRelicItem> GetAll()
        {
            return _relicItems;
        }

        public List<VBattleRelic> GetRelics()
        {
            return _relicItems.Select(item => item.Relic as VBattleRelic).ToList();
        }

        public void AddItems(IEnumerable<VBattleDebugRelicItem> items)
        {
            foreach (var item in items)
            {
                item.Unselect();
                _relicItems.Add(item);
                item.transform.SetParent(content);
            }
        }
    }
}