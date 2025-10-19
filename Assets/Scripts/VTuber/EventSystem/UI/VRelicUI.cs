using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Relic; // Add this using statement
// using TMPro; 

public class VRelicUI : MonoBehaviour 
{
    [SerializeField] private Image relicIcon;
    [SerializeField] private TextMeshProUGUI relicNameText;

    // The parameter is now the correct type
    public void Initialize(VRelic relicData) // <-- Use the base class here
    {
        if (relicData != null)
        {
            relicIcon.sprite = relicData.Icon;
            relicNameText.text = relicData.GetRelicName();
        }
    }
}