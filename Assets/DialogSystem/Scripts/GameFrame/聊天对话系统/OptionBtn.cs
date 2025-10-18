using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VTuber.Character;
using VTuber.Core.Managers;
using VTuber.Core.SE;

public class OptionBtn:MonoBehaviour,IPointerEnterHandler,IPointerExitHandler 
{
    public Button btn;
    private string optionDescription;
    public Text btnText;
    public void SetBtn(DialogContent dc, VCharacter character)
    {
        optionDescription = dc.optionDescription;
        btnText.text = dc.context;
        
        btn.onClick.AddListener(() =>
        {
            dc.AppleEffects(character);
        });
        
        btn.onClick.AddListener(() =>
        {
            VAudioPlayer.Instance.PlayStaticSFX(VSFXType.Selection);
            DialogSystem.Instance.CreateDialog(dc);
            DialogSystem.Instance.HideOptionDescription();
        });
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        DialogSystem.Instance.ShowOptionDescription(optionDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DialogSystem.Instance.HideOptionDescription();
    }
}
