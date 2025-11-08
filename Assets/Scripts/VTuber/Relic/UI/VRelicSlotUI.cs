using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
        public VRelic Relic { get; private set; }

        public uint BattleID { get; private set; }
        public bool IsAdditional { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            IsAdditional = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            descriptionObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            descriptionObject.SetActive(false);
        }

        public void SetIsAdditional(bool isAdditional)
        {
            IsAdditional = isAdditional;
        }

        public void Initialize(VRelic relic, bool displayValue)
        {
            Relic = relic;
            if (displayValue && !relic.IsPermanent)
            {
                layer.gameObject.SetActive(true);
                UpdateValue();
            }

            icon.sprite = relic.Icon;
            icon.gameObject.SetActive(true);
            description.text = Relic.Description;
            BattleID = (relic as VBattleRelic)?.BattleID ?? 10000;
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
    }
}