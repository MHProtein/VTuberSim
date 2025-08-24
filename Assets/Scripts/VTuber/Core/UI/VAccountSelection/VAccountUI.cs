using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VTuber.Core.Foundation;
using VTuber.Core.RaisingEffect;
using VTuber.Reincarnation;

namespace VTuber.BattleSystem.Core.UI.VAccountSelection
{
    public class VAccountUI : VUIBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image background;
        [SerializeField] private TMP_Text name;
        [SerializeField] private List<Image> cardIcons;

        [SerializeField] private Transform attributeGrids;
        [SerializeField] private int maxAttribtues;
        [SerializeField] private GameObject attributePrefab;
        [SerializeField] private Sprite ellipsisIcon;
        
        public VAccount Account => _account;
        private VAccount _account;
        private VAccountSelectionMenu _menu;
        
        private List<VAttributeEffectUI> attributeEffectUIs;
        private bool _selected;
        private bool _selectable;
        private bool _picked;

        public void Initialize(VAccountSelectionMenu menu, VAccount account)
        {
            _picked = false;
            _selectable = true;
            _menu = menu;
            _account = account;
            
            for (int i = 0; i < cardIcons.Count; i++)
            {
                cardIcons[i].gameObject.SetActive(false);
            }
            
            for (int i = 0; i < account.Cards.Count; i++)
            {
                //cardIcons[i].sprite = account.Cards[i].icon;
                cardIcons[i].gameObject.SetActive(true);
                if (i == cardIcons.Count - 1 && i < account.Cards.Count - 1)
                {
                    cardIcons[i].sprite = ellipsisIcon;
                }
            }

            attributeEffectUIs = new List<VAttributeEffectUI>();
            for (int i = 0; i < maxAttribtues; i++)
            {
                if (i >= account.Effects.Count)
                    return;
                var effect = account.Effects[i];
                var attributeEffect = effect as IAttributeEffect;
                if (attributeEffect != null)
                {
                    var attributeEffectUI = Instantiate(attributePrefab, attributeGrids).GetComponent<VAttributeEffectUI>();
                    attributeEffectUI.Initialize(effect, account.EffectItems[i].level);
                    attributeEffectUIs.Add(attributeEffectUI);
                }
            }

            if (account.Effects.Count > maxAttribtues)
            {
                attributeEffectUIs[attributeEffectUIs.Count - 1].SetEllipsis(ellipsisIcon);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_picked)
            {
                if (_selected)
                {
                    _selected = false;
                    background.color = Color.white;
                    _picked = false;
                    _menu.UnpickAccount(this);
                }
                else
                {
                    _selected = true;
                    background.color = Color.cyan;
                    _menu.OnSelected(this);
                }

                return;
            }
            
            if (!_selectable)
                return;
            if (_selected)
            {
                _selected = false;
                background.color = Color.white;
                _picked = true;
                _menu.PickAccount(this);
            }
            else
            {
                _selected = true;
                background.color = Color.cyan;
                _menu.OnSelected(this);
            }
        }
        
        public void Deselect()
        {
            _selected = false;
            
            if (!_selectable)
                return;
            background.color = Color.white;
        }
        
        public void SetSelectable(bool value)
        {
            if (_picked)
                return;
            _selectable = value;
            if (!value)
                _selected = false;
            background.color = value ? Color.white : Color.grey;
        }
    }
}