using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Core.Foundation;
using VTuber.Core.ScriptSystem;

namespace VTuber.BattleSystem.Core.UI
{
    public class VScriptSelectionUI  : VUIBehaviour
    {
        
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text name;
        [SerializeField] private TMP_Text description;
        [Header("Coop")] [SerializeField] private Transform coopTransform;
        [Header("Coop")] [SerializeField] private GameObject coopPrefab;
        
        public void ShowScript(VScriptConfiguration script)
        {
            icon.sprite = script.icon;
            name.text = script.scriptName;
            description.text = script.description;
            foreach (var coop in script.coops)
            {
                var coopObj = Instantiate(coopPrefab, coopTransform);
                var coopScript = coopObj.GetComponent<VScriptCoop>();
                coopScript.icon.sprite = coop.Icon;
                coopScript.coopName.text = coop.Name;
            }
        }
    }
}