using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VTuber.Core.Foundation;

namespace VTuber.CoopSystem.UI
{
    public class VCooperatorUI : VUIBehaviour
    {
        public uint Id { get; private set; }
        [SerializeField] private Image pfp;
        [SerializeField] private TMP_Text cooperatorName;
        [SerializeField] private TMP_Text coopLevel;
        
        public void SetCooperator(VCooperator cooperator)
        {
            Id = cooperator.Id;
            pfp.sprite = cooperator.configuration.Icon;
            cooperatorName.text = cooperator.configuration.Name;
            coopLevel.text = cooperator.CurrentCoopLevel.levelName;
        }

        public void UpdateValue(VCooperator cooperator)
        {
            coopLevel.text = cooperator.CurrentCoopLevel.levelName;
        }
    }
}