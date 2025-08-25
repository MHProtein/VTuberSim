using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VTuber.Core.Foundation;

namespace VTuber.Relic.UI
{
    public class VRelicSlotUI : VUIBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject descriptionObject;
        [SerializeField] private TMP_Text description;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text layer;
        public VRelic Relic => _relic;
        private VRelic _relic;
        public bool IsAdditional => _isAdditional;
        private bool _isAdditional;

        protected override void Awake()
        {
            base.Awake();
            _isAdditional = false;
        }

        public void SetIsAdditional(bool isAdditional)
        {
            _isAdditional = isAdditional;
        }
        
        public void Initialize(VRelic relic)
        {
            _relic = relic;
            if (!relic.IsPermanent)
            {
                layer.gameObject.SetActive(true);
                UpdateValue();
            }
            icon.sprite = relic.Icon;
            icon.gameObject.SetActive(true);
            description.text = _relic.Description;
        }

        public bool HasRelic()
        {
            return _relic is not null;
        }

        public void Clear()
        {
            _relic = null;
            layer.gameObject.SetActive(false);
            icon.gameObject.SetActive(false);
        }

        public void UpdateValue()
        {
            layer.text = _relic.Layer.ToString();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            descriptionObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            descriptionObject.SetActive(false);
        }
    }
}