using UnityEngine;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;


namespace VTuber.Dialogue
{
    public class AutoAdvanceButtonController : MonoBehaviour
    {
        public LinePresenter linePresenter;
        public TMPro.TextMeshProUGUI buttonText;
        public DialogueRunner dialogueRunner;
        public void ToggleAutoAdvance()
        {
            if (linePresenter == null) {
                Debug.LogWarning("LinePresenter is not assigned!");
                return;
            }

            linePresenter.autoAdvance = !linePresenter.autoAdvance;
            
            // 立即跳过当前等待，推进对话
            dialogueRunner.RequestNextLine();
            
            if (buttonText != null) {
                buttonText.text = linePresenter.autoAdvance ? "Skip ON" : "Skip OFF";
            }

            Debug.Log("AutoAdvance: " + linePresenter.autoAdvance);
        }
    }


}