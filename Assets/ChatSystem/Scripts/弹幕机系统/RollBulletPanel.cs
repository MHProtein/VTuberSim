using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
[System.Serializable]


public class RollBulletPanel : SingletonMono<RollBulletPanel>
{
    [Header("系统设置")]
     public float ScrollSpeed = 100f;
     public int maxBulletOnScreen = 50;
     public bool enableAutoClear = true;
    
    [Header("引用")]
    [SerializeField] private BulletTrackManager trackManager;
    
    
    private Queue<BulletData> bulletQueue = new Queue<BulletData>();
    public int currentBulletCount = 0;


    public void ShowMe()
    {
        this.gameObject.SetActive(true);
    }

    public void HideMe()
    {
        this.gameObject.SetActive(false);
        ClearAll();
    }

    private void Start()
    {
        InitializeSystem();
    }
    
    private void InitializeSystem()
    {
        
        trackManager.Initialize();
        BulletSystem.Instance.onBulletGenerate += HandleBulletGenerated;
    }
    
    
    
    private void HandleBulletGenerated(BulletInfo bulletInfo)
    {
        BulletData bulletData = new BulletData();
        bulletData.content = bulletInfo.bulletContent;
        bulletData.speed = ScrollSpeed;
        // 加入队列
        bulletQueue.Enqueue(bulletData);
        ProcessBulletQueue();
    }
    
    private void ProcessBulletQueue()
    {
        while (bulletQueue.Count > 0 && currentBulletCount < maxBulletOnScreen)
        {
            BulletData data = bulletQueue.Dequeue();
            
            if (trackManager.TrySpawnBullet(data))
            {
                currentBulletCount++;
            }
            else
            {
                // 生成失败，可以重新入队或丢弃
                bulletQueue.Enqueue(data);
                break; // 避免无限循环
            }
        }
    }
    
    // 公共方法
    public void AddBullet(string content, Color? color = null, int fontSize = 24)
    {
        BulletData data = new BulletData
        {
            content = content,
            color = color ?? Color.white,
            fontSize = fontSize,
            speed = ScrollSpeed
        };
        
        bulletQueue.Enqueue(data);
    }
    
    public void SetScrollSpeed(float speed)
    {
        ScrollSpeed = Mathf.Max(1f, speed);
    }
    
    public void SetMaxBullet(int maxCount)
    {
        maxBulletOnScreen = maxCount;
    }
    
    public void ClearAll()
    {
        trackManager.ClearAllBullet();
        bulletQueue.Clear();
        currentBulletCount = 0;
    }
    
    private void OnDestroy()
    {
        BulletSystem.Instance.onBulletGenerate -= HandleBulletGenerated;
    }
}