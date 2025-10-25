using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.Buff;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VBuffDetailsUI : VUIBehaviour
    {
        [SerializeField] private TMP_Text layer;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;

        public void SetBuff(VBuffItem buff)
        {
            if (!buff.buff.IsStackable())
                layer.gameObject.SetActive(false);
            nameText.text = buff.buff.GetBuffName();
            icon.sprite = buff.buff.Icon;
        }

        public void UpdateBuff(string layerStr, string description)
        {
            layer.text = layerStr;
            descriptionText.text = description;
        }
    }
}