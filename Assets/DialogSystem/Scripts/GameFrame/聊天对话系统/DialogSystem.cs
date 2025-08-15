using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VTuber.Character;
using VTuber.Core.Managers;

public class DialogSystem : SingletonMono<DialogSystem>
{
    public Transform canvas;

    private Dialog currentDialog;
    private bool canContinue = false;

    public RectTransform dialogRoot;
    public GameObject dialogPrefab_L;
    public GameObject dialogPrefab_L_Image;
    public GameObject dialogPrefab_R;
    public GameObject dialogPrefab_R_Image;
    
    public RectTransform optionBtnRoot;
    public List<GameObject> optionBtnPrefab;
    private Stack<GameObject> optionBtnStack = new Stack<GameObject>();
    
    
    private UnityAction<Dialog> onDialogFinished;
    public event UnityAction<Dialog> OnDialogFinished;

    public Text optionDescription;
    public GameObject descriptionObj;
    
    public ScrollRect scrollRect;
    public RectTransform scrollContent;
    public float scrollDelay = 0.1f;
    
    private bool shouldScrollToBottom = false;
    private float lastContentHeight;

    public Button AutoBtn;
    private bool auto = false;
    public float autoDelay = 0.5f;
    public Button PauseBtn;
    public Button SkipBtn;
    private bool skip = false;
    public float skipDelay = 0.1f;
    
    private VCharacter _character;
    private DialogObj currentDialogObj;
    protected override void Awake()
    {
        base.Awake();

        //初始化选项按钮
        foreach (var optionBtn in optionBtnPrefab)
        {
            GameObject obj = Instantiate(optionBtn,canvas,false);
            obj.SetActive(false);
            optionBtnStack.Push(obj);
        }
        
        //记录对话内容大小
        lastContentHeight = scrollContent.rect.height;
        
        
        //初始化功能按钮
        AutoBtn.onClick.AddListener(() =>
        {
            if (!auto)
            {
                auto = true;
                PauseSkip();
                PauseBtn.interactable = true;
                StartCoroutine(AutoDialog());
            }
        });
        PauseBtn.onClick.AddListener(() =>
        {
            PauseAuto();
            PauseSkip();
        });
        
        SkipBtn.onClick.AddListener(() =>
        {
            if (!skip)
            {
                skip = true;
                PauseAuto();
                PauseBtn.interactable = true;
                StartCoroutine(SkipDialog());
            }
        });

        
        HideMe();
        
        
    }

    private void Update()
    {
        if (canContinue && Input.GetKeyDown(KeyCode.Space))
        {
            if (auto)
            {
                PauseAuto();
            }
            ContinueDialog();
        }
    }

    public void ShowMe(VCharacter character)
    {
        this.gameObject.SetActive(true);
        canContinue = true;
        _character = character;
        //StartCoroutine(StartDialog());
    }

    public void HideMe()
    {
        //StopCoroutine(StartDialog());
        this.gameObject.SetActive(false);
        
    }

    public void LoadDialog(string dialogName)
    {
        ClearBtns();
        ClearDialogs();
        Dialog dialog = VDialogResourcesManager.Instance.TryGetDialog(dialogName);
        dialog.index = 0;
        
        if(!dialog.loaded)
            dialog.InitDialog();
        
        currentDialog = dialog;
        
        EnableFunctionBtn(true);
        PauseBtn.interactable = false;
    }
    
    private void ClearDialogs()
    {
        if (dialogRoot&&dialogRoot.childCount > 0)
            DestroyAllChildren(dialogRoot);
    }

    private void ClearBtns()
    {
        if (optionBtnRoot && optionBtnRoot.childCount > 0)
        {
            for (int i = optionBtnRoot.childCount - 1; i >= 0; i--)
            {
                optionBtnStack.Push(optionBtnRoot.GetChild(i).gameObject);
                optionBtnRoot.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                optionBtnRoot.GetChild(i).gameObject.SetActive(false);
                optionBtnRoot.GetChild(i).SetParent(null);
            }
        }
    }

    public void DestroyAllChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

    // IEnumerator StartDialog()
    // {
    //     while (true)
    //     {
    //         if (canContinue && Input.anyKeyDown)
    //         {
    //             ContinueDialog();
    //             yield return null;
    //         }
    //     }
    // }

    public void ContinueDialog()
    {
        DialogContent dc=currentDialog.contentDic[currentDialog.index];
        

        if (dc.ifOption)
        {
            canContinue = false;
            PauseAuto();
            PauseSkip();
            CreateOptionBtn(dc);
        }
        else
        {
            if (canContinue)
            {
                CreateDialog(dc);
            }
        }
        
    }

    public void CreateDialog(DialogContent dc)
    {
        if (!dc.ifOption && currentDialogObj != null)
        {
            if (dc.effectID != -1)
            {
                var effect = VResourcesManager.Instance.CreateRaisingEffectByID((uint)dc.effectID, dc.effectParameter, dc.effectParameter);
                if(effect is not null)
                    effect.ApplyEffect(_character);
            }
        }
        
        GameObject dialogObj;
        if (dc.ifPlayer)
        {
            if (dc.ifImage)
            {
                dialogObj = Instantiate(dialogPrefab_R_Image,canvas,false);
            }
            else
            {
                dialogObj=Instantiate(dialogPrefab_R,canvas,false);
            }

            
        }
        else
        {
            if (dc.ifImage)
            {
                dialogObj = Instantiate(dialogPrefab_L_Image,canvas,false);
            }
            else
            {
                dialogObj=Instantiate(dialogPrefab_L,canvas,false);
            }
           
        }
        
        currentDialogObj = dialogObj.GetComponent<DialogObj>();
        currentDialogObj.ShowDialog(dc);
        currentDialogObj.transform.SetParent(dialogRoot);    
        Canvas.ForceUpdateCanvases();
        
        AdjustScrollView();

        ClearBtns();
        canContinue = true;
        if (dc.nextId == -1)
        {
            OnDialogFinished?.Invoke(currentDialog);
            EnableFunctionBtn(false);
            canContinue = false;
        }
        currentDialog.index=dc.nextId;
    }

    public GameObject GetBtn()
    {
        return optionBtnStack.Pop();
    }

    private void CreateOptionBtn(DialogContent dc)
    {
        GameObject optionBtnObj = GetBtn();
        optionBtnObj.SetActive(true);
        optionBtnObj.transform.SetParent(optionBtnRoot);
        optionBtnObj.GetComponent<OptionBtn>().SetBtn(dc, _character);
        currentDialog.index++;
        ContinueDialog();
    }

    public void ShowOptionDescription(string description)
    {
        optionDescription.text = description;
        descriptionObj.SetActive(true);
    }

    public void HideOptionDescription()
    {
        optionDescription.text = "";
        descriptionObj.SetActive(false);
    }

    public void AdjustScrollView()
    {
        // 如果需要滚动且内容超出视图
        if ( scrollContent.rect.height > scrollRect.viewport.rect.height)
        {
            StartCoroutine(ScrollToBottomAfterDelay());
        }
    }
    IEnumerator ScrollToBottomAfterDelay()
    {
        yield return new WaitForSeconds(scrollDelay);
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    IEnumerator AutoDialog()
    {
        while (auto)
        {
            ContinueDialog();
            yield return new WaitForSeconds(autoDelay);
        }
        
    }

    private void PauseAuto()
    {
        auto = false;
        PauseBtn.interactable=false;
    }

    IEnumerator SkipDialog()
    {
        while (skip)
        {
            ContinueDialog();
            yield return new WaitForSeconds(skipDelay);
        }
    }

    private void PauseSkip()
    {
        skip = false;
        PauseBtn.interactable = false;
    }

    private void EnableFunctionBtn(bool enable)
    {
        AutoBtn.interactable = enable;
        SkipBtn.interactable = enable;
        PauseBtn.interactable = enable;
        if (!enable)
        {
            PauseAuto();
            PauseSkip();
        }
    }

    public void SetCanContinue(bool p0)
    {
        canContinue = p0;
    }
}
