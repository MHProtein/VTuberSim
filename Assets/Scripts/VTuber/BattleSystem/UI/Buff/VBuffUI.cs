using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.BattleSystem.Buff;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    class VBuffUI : VUIBehaviour
    {
        [HideInInspector]public uint id;
        [HideInInspector]public bool isPermanent;
        [SerializeField] private TMP_Text layer;
        [SerializeField] private TMP_Text nameText;
        
        private VBuffItem _buffItem;

        public void SetBuff(VBuffItem buff)
        {
            id = buff.Id;
            _buffItem = buff;
            isPermanent = buff.buff.IsPermanent;
            if(isPermanent)
                layer.gameObject.SetActive(false);
            nameText.text = buff.buff.GetBuffName();
        }
        
        public void SetText(int value)
        {
            layer.text = value.ToString();
            if (_buffItem.buff.BuffType == BuffType.Persistent)
                layer.text += "回合";
        }
    }
}