using UnityEngine;
namespace VTuber.Screenplay
{
    public class ScoreSystem : MonoBehaviour
    {
        // 输入数据
        public float totalPowerPanel;       // 总面板
        public int fanCount;                // 粉丝数
        public int highestCaptainCount;     // 最高舰长数
        public float missionBonus;          // 完成目标奖励（直接加分）

        // 系数设定（可在Inspector中设置）
        public float powerMultiplier = 1.0f;
        public float fanMultiplier = 0.5f;
        public float captainMultiplier = 2.0f;

        // 最终得分
        private float finalScore;

        void Start()
        {
            CalculateScore();
            Debug.Log("最终得分为: " + finalScore);
        }

        public void CalculateScore()
        {
            finalScore = totalPowerPanel * powerMultiplier
                         + fanCount * fanMultiplier
                         + highestCaptainCount * captainMultiplier
                         + missionBonus;
        }

        // 获取分数（可被UI或其他系统调用）
        public float GetScore()
        {
            return finalScore;
        }
    }

}