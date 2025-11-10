using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VTuber.BattleSystem.Buff;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    internal class VBuffUI : VUIBehaviour, IPointerClickHandler
    {
        [HideInInspector] public uint id;
        [HideInInspector] public bool isPermanent;
        [SerializeField] private TMP_Text layer;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text nameText;

        private VBuffItem _buffItem;
        private VBuffDetailsUI _detailsUI;
        public Action onClick;
        public uint ConfigID => _buffItem.ConfigId;


        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke();
        }

        public void SetBuff(VBuffItem buff, VBuffDetailsUI details)
        {
            id = buff.Id;
            _buffItem = buff;
            _detailsUI = details;
            isPermanent = buff.buff.IsPermanent;
            if (!buff.buff.IsStackable())
                layer.gameObject.SetActive(false);
            nameText.text = buff.buff.GetBuffName();
            if (buff.buff.Icon is not null)
                icon.sprite = buff.buff.Icon;

            _detailsUI.SetBuff(buff);
        }

        public void SetText(int value)
        {
            layer.text = value.ToString();
            if (_buffItem.buff.BuffType == BuffType.Persistent)
                layer.text += "回合";
            _detailsUI.UpdateBuff(layer.text, _buffItem.buff.GetDescription(value));
        }

        public void Clear()
        {
            Destroy(_detailsUI.gameObject);
        }
    }
}