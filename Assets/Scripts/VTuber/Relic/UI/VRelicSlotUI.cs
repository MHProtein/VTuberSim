using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VTuber.Core.Foundation;
using VTuber.Core.UI;

namespace VTuber.Relic.UI
{
    public class VRelicSlotUI : VUIBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject descriptionObject;
        [SerializeField] private TMP_Text description;
        [SerializeField] private Image icon;
        [SerializeField] private Image background;
        [SerializeField] private TMP_Text layer;
        private bool _shouldShowDescription = true;
        public VRelic Relic { get; private set; }

        public uint BattleID { get; private set; }
        public bool IsAdditional { get; private set; }
        public Image Icon => icon;

        private bool _isPermanentDescriptionShown = false;
        private VRelicMenu _relicMenu;
        public bool IsDisplayValue => layer.gameObject.activeSelf;

        protected override void Awake()
        {
            base.Awake();
            IsAdditional = false;
        }
        
        public void SetShouldShowDescription(bool shouldShowDescription)
        {
            _shouldShowDescription = shouldShowDescription;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_isPermanentDescriptionShown)
                return;
            if (_relicMenu)
            {
                _relicMenu.SetDescription(Relic);
                return;
            }
            if(_shouldShowDescription)
                descriptionObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isPermanentDescriptionShown || !_shouldShowDescription)
                return;
            if (_relicMenu)
            {
                _relicMenu.SetDescription(null);
                return;
            }
            if(_shouldShowDescription)
                descriptionObject.SetActive(false);
        }

        public void SetIsAdditional(bool isAdditional)
        {
            IsAdditional = isAdditional;
        }

        public void Initialize(VRelic relic, bool displayValue, VRelicMenu relicMenu = null)
        {
            Relic = relic;
            _relicMenu = relicMenu;
            if (displayValue && !relic.IsPermanent)
            {
                layer.gameObject.SetActive(true);
                UpdateValue();
            }

            icon.sprite = relic.Icon;
            icon.gameObject.SetActive(true);
            description.text = Relic.Description;

            if (relic is VBattleRelic battleRelic)
                BattleID = battleRelic.BattleID;
        }

        public bool HasRelic()
        {
            return Relic is not null;
        }

        public void Clear()
        {
            Relic = null;
            layer.gameObject.SetActive(false);
            icon.gameObject.SetActive(false);
        }

        public void UpdateValue()
        {
            layer.text = Relic.Layer.ToString();
        }

        public void SetBackgroundColor(Color color)
        {
            if(background)
                background.color = color;
        }

        public void ShowDescriptionPermenant()
        {
            _isPermanentDescriptionShown = true;
            descriptionObject.SetActive(true);
        }

        public void SetAlpha(float f)
        {
            VUIUtils.SetImageAlpha(icon, f);
        }

        public void DisplayValue(bool isDisplay)
        {
            layer.gameObject.SetActive(isDisplay);
        }

        public void DisplayBackground(bool isDisplay)
        {
            background.gameObject.SetActive(isDisplay);
        }
    }
}