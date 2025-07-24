using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GradeThreshold
{
    public float minScore;  // 最小得分（包含）
    public float maxScore;  // 最大得分（包含）
    public string gradeName;  // 评级名称
}

public class ScoreSystem : MonoBehaviour
{
    [Header("评分输入")]
    public float totalPowerPanel;
    public int fanCount;
    public int highestCaptainCount;
    public float missionBonus;

    [Header("评分系数")]
    public float powerMultiplier = 1.0f;
    public float fanMultiplier = 0.5f;
    public float captainMultiplier = 2.0f;

    [Header("评级设置")]
    public List<GradeThreshold> gradeThresholds = new List<GradeThreshold>();

    private float finalScore;
    private string currentGrade;

    void Start()
    {
        CalculateScore();
        Debug.Log("最终得分为: " + finalScore);
        Debug.Log("评级为: " + currentGrade);
    }

    public void CalculateScore()
    {
        finalScore = totalPowerPanel * powerMultiplier
                     + fanCount * fanMultiplier
                     + highestCaptainCount * captainMultiplier
                     + missionBonus;

        EvaluateGrade();
    }

    private void EvaluateGrade()
    {
        currentGrade = "未评级";  // 默认值
        foreach (var threshold in gradeThresholds)
        {
            if (finalScore >= threshold.minScore && finalScore <= threshold.maxScore)
            {
                currentGrade = threshold.gradeName;
                break;
            }
        }
    }

    public float GetScore()
    {
        return finalScore;
    }

    public string GetGrade()
    {
        return currentGrade;
    }
}