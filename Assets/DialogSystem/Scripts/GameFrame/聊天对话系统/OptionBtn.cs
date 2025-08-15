using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VTuber.Character;
using VTuber.Core.Managers;

public class OptionBtn:MonoBehaviour,IPointerEnterHandler,IPointerExitHandler 
{
    public Button btn;
    private string optionDescription;
    public Text btnText;
    public void SetBtn(DialogContent dc, VCharacter character)
    {
        optionDescription = dc.optionDescription;
        btnText.text = dc.context;
        
        if(dc.effectID != -1)
            btn.onClick.AddListener(() =>
            {
                var effect = VResourcesManager.Instance.CreateRaisingEffectByID((uint)dc.effectID, dc.effectParameter, dc.effectParameter);
                if(effect is not null)
                    effect.ApplyEffect(character);
            });
        
        btn.onClick.AddListener(() =>
        {
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
