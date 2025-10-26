using UnityEngine;
using UnityEngine.UI;
using VTuber.Relic;
using TMPro;

//this script have benn dispute
public class VRelicUI : MonoBehaviour
{
    [SerializeField] private Image relicIcon;
    [SerializeField] private TextMeshProUGUI relicNameText;

    public void Initialize(VRelic relicData)
    {

        Debug.Log($"[DEBUG] VRelicUI.Initialize called for '{relicData.GetRelicName()}' on GameObject: {this.name}");

        if (relicIcon == null)
        {
            Debug.LogError("[DEBUG] VRelicUI Error: The 'relicIcon' Image is NOT ASSIGNED in the prefab's Inspector!", this.gameObject);
            return;
        }
        
        if (relicData.Icon == null)
        {
            Debug.LogWarning($"[DEBUG] Relic '{relicData.GetRelicName()}' has a NULL icon sprite!");
        }

        relicIcon.sprite = relicData.Icon;
        if (relicNameText != null)
        {
            relicNameText.text = relicData.GetRelicName();
        }
    }
}