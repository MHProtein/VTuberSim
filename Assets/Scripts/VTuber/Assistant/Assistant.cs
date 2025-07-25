using UnityEngine;
using System.Collections.Generic;
namespace VTuber.Assistant
{

    [System.Serializable]
    public class Assistant
    {
        public string name;
        public int level;
        public float favorability;
        public Sprite portrait;

        // 构造函数
        public Assistant(string name, int level, float favorability, Sprite portrait)
        {
            this.name = name;
            this.level = level;
            this.favorability = favorability;
            this.portrait = portrait;
        }

        // 提升好感度
        public void IncreaseFavorability(float amount)
        {
            favorability += amount;
            CheckLevelUp();
        }

        // 检查是否升级
        private void CheckLevelUp()
        {
            if (favorability >= GetRequiredFavorabilityForNextLevel())
            {
                level++;
                OnLevelUp();
            }
        }

        // 获取下一级所需好感度（可自定义规则）
        private float GetRequiredFavorabilityForNextLevel()
        {
            return level * 100f; // 示例：每一级需要多100点好感
        }

        // 升级时触发的事件
        private void OnLevelUp()
        {
            Debug.Log($"{name} 升级至等级 {level}！");
            // TODO: 解锁新事件或其他内容
        }
    }

    public class AssistantManager : MonoBehaviour
    {
        public List<Assistant> assistants;
        public Dictionary<string, List<string>> assistantEvents = new Dictionary<string, List<string>>();

        private void Start()
        {
            // 示例初始化
            Assistant newAssistant = new Assistant("小艾", 1, 0f, null); // 立绘需手动设置
            assistants.Add(newAssistant);

            // 添加事件表
            assistantEvents["小艾"] = new List<string> { "事件1", "事件2" };
        }

        // 触发协助者事件
        public void TriggerEvent(string assistantName)
        {
            if (assistantEvents.ContainsKey(assistantName))
            {
                foreach (var evt in assistantEvents[assistantName])
                {
                    Debug.Log($"触发事件：{evt}");
                    // TODO: 实际事件逻辑
                }
            }
        }

        // 解锁新等级后的事件
        public void UnlockLevelEvents(Assistant assistant)
        {
            Debug.Log($"{assistant.name} 解锁了新的等级事件！");
            // TODO: 根据等级添加或解锁事件
        }
    }
}