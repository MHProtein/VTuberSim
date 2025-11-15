using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VTuber.Character;
using VTuber.Core.SE;
using VTuber.RaisingAnimationSystem;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;
using Random = UnityEngine.Random;

public class DialogSystem : SingletonMono<DialogSystem>, IPointerClickHandler
{
    public Transform canvas;
    public Text dialogName;
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
    public event UnityAction<int> OnLineFinished;

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
    private bool paused = false;
    private bool shouldEnd;
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
    
    public void SetPaused(bool paused)
    {
        this.paused = paused;
    }

    public void ShowMe(VCharacter character)
    {
        this.gameObject.SetActive(true);
        canContinue = true;
        _character = character;
        AdjustScrollView();
        //StartCoroutine(StartDialog());
        VAudioPlayer.Instance.PlayBGM(VBGMType.Dialog);
    }

    public void HideMe()
    {
        //StopCoroutine(StartDialog());
        this.gameObject.SetActive(false);
        VAudioPlayer.Instance.StopBGM();
    }

    public void LoadDialog(string dialogName)
    {
        ClearBtns();
        ClearDialogs();
        Dialog dialog = VResourcesManager.Instance.TryGetDialog(dialogName);
        dialog.index = 0;
        
        if(!dialog.loaded)
            dialog.InitDialog();
        
        currentDialog = dialog;
        
        EnableFunctionBtn(true);
        PauseBtn.interactable = false;
        this.dialogName.text = dialog.dialogName;
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
            
            List<DialogContent> options = new List<DialogContent>();
            var option = dc;
            while (option.ifOption)
            {
                options.Add(option);
                if (option.nextId == -1)
                {
                    break;
                }
                option = currentDialog.contentDic[++currentDialog.index];
            }

            var showingOptions = new List<DialogContent>();
            if (options.Count > 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    int index = Random.Range(0, options.Count);
                    showingOptions.Add(options[index]);
                    options.RemoveAt(index);
                }
                showingOptions.Sort((content, dialogContent) => content.id.CompareTo(dialogContent.id));
            }
            else
            {
                showingOptions = options;
            }
            
            var tempIndex = currentDialog.index;
            
            foreach (var op in showingOptions)
            {
                CreateOptionBtn(op);
            }

            currentDialog.index = tempIndex;
        }
        else
        {
            if (canContinue)
            {
                CreateDialog(dc);
            }
        }
        
    }

    public void CreateDialog(DialogContent dc, bool isLoadedSkip = false)
    {
        if (!isLoadedSkip && currentDialogObj != null)
        {
            dc.AppleEffects(_character);
        }
        
        GameObject dialogObj;
        if (dc.ifPlayer)
        {
            if (dc.ifImage)
            {
                dialogObj = Instantiate(dialogPrefab_R_Image, canvas, false);
            }
            else
            {
                dialogObj = Instantiate(dialogPrefab_R, canvas, false);
            }

            
        }
        else
        {
            if (dc.ifImage)
            {
                dialogObj = Instantiate(dialogPrefab_L_Image, canvas, false);
            }
            else
            {
                dialogObj = Instantiate(dialogPrefab_L, canvas, false);
            }
        }
        
        currentDialogObj = dialogObj.GetComponent<DialogObj>();
        currentDialogObj.ShowDialog(dc);
        currentDialogObj.transform.SetParent(dialogRoot);    
        currentDialogObj.transform.localScale = Vector3.one * 0.85f;  
        Canvas.ForceUpdateCanvases();
        
        AdjustScrollView();

        ClearBtns();
        canContinue = true;
        
        if(!isLoadedSkip) 
            OnLineFinished?.Invoke(dc.id);
        
        if (dc.nextId == -1)
        {
            shouldEnd = true;
        }
        currentDialog.index = dc.nextId;

        if(VRaisingAnimationSystem.Instance.HasAnimationRequests())
        {
            PauseAuto();
            PauseSkip();
            SetPaused(true);
            var tempAuto = auto;
            var tempSkip = skip;
            VRaisingAnimationSystem.Instance.ExecuteAnimations(() =>
            {
                auto = tempAuto;
                skip = tempSkip;
                SetPaused(false);
            });
        }
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
        optionBtnObj.transform.localScale = Vector3.one * 0.85f;  
        currentDialog.index++;
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
            if (shouldEnd)
            {
                PauseAuto();
                break;
            }

            // stay paused until unpaused
            while (paused)
                yield return null;

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
            if (shouldEnd)
            {
                PauseSkip();
                break;
            }
            while (paused)
                yield return null;
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (shouldEnd)
        {
            shouldEnd = false;
            OnDialogFinished?.Invoke(currentDialog);
            EnableFunctionBtn(false);
            canContinue = false;
            return;
        }
        if (!paused && canContinue)
        {
            if (auto)
            {
                PauseAuto();
            }
            ContinueDialog();
        }
    }

    public void SkipTo(List<int> executedLines)
    {
        if (executedLines is null)
            return;
        foreach (var executedLine in executedLines)
        {
            CreateDialog(currentDialog.contentDic[executedLine], true);
        }
        currentDialog.index = currentDialog.contentDic[executedLines[executedLines.Count - 1]].nextId;
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
