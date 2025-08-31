using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VTuber.BattleSystem.Buff;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    class VBuffUI : VUIBehaviour, IPointerClickHandler
    {
        public uint ConfigID => _buffItem.ConfigId;
        [HideInInspector]public uint id;
        [HideInInspector]public bool isPermanent;
        [SerializeField] private TMP_Text layer;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text nameText;
        private VBuffDetailsUI _detailsUI;
        
        private VBuffItem _buffItem;
        public Action onClick;

        public void SetBuff(VBuffItem buff, VBuffDetailsUI details)
        {
            id = buff.Id;
            _buffItem = buff;
            _detailsUI = details;
            isPermanent = buff.buff.IsPermanent;
            if(!buff.buff.IsStackable())
                layer.gameObject.SetActive(false);
            nameText.text = buff.buff.GetBuffName();
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


        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke();
        }
    }
}