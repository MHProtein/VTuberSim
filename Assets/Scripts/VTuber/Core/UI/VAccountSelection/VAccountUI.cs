using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Core.Foundation;
using VTuber.Core.RaisingEffect;
using VTuber.Reincarnation;

namespace VTuber.BattleSystem.Core.UI.VAccountSelection
{
    public class VAccountUI : VUIBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text name;
        [SerializeField] private List<Image> cardIcons;

        [SerializeField] private Transform attributeGrids;
        [SerializeField] private int maxAttribtues;
        [SerializeField] private GameObject attributePrefab;
        [SerializeField] private Sprite ellipsisIcon;
        private VAccount _account;
        
        private List<VAttributeEffectUI> attributeEffectUIs;

        public void Initialize(VAccountSelectionMenu menu, VAccount account)
        {
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
                    attributeEffectUI.Initialize(effect);
                    attributeEffectUIs.Add(attributeEffectUI);
                }
            }

            if (account.Effects.Count > maxAttribtues)
            {
                attributeEffectUIs[attributeEffectUIs.Count - 1].SetEllipsis(ellipsisIcon);
            }
        }
    }
}