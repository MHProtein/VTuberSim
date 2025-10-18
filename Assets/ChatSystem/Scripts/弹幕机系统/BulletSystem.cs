using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;
using Random = UnityEngine.Random;

public class BulletInfo
{
    public string bulletContent;
    public string senderName;
    public int senderIconId;
}



public class EffectBullet
{
    public VBattleEventKey bulletState;
    public int bulletCount;
}

public class BulletSystem : VSingletonMonobehaviour<BulletSystem>
{
    public event UnityAction<BulletInfo> onBulletGenerate;
    [SerializeField] private Dictionary<VBattleEventKey, BulletList> bulletListsDic = new Dictionary<VBattleEventKey, BulletList>();
    private void InitBulletSystem()
    {
        foreach (var bulletList in bulletListsDic)
        {
            bulletList.Value.InitBulletList();
        }
    }
    private BulletGenerator bulletGenerator = new BulletGenerator();
    
    [Header("弹幕机控制参数")]
    [Header("弹幕生成时间间隔范围")]
    public float timeMax;
    public float timeMin;
    private float timeBetweenBullet;
    [Header("弹幕生成开关")]
    public bool ifGenerate=false;
    
    //运行时变量
    private float generateTimer=0;
    private int effectBulletCount;
    private Queue<EffectBullet> effectBulletQueue=new Queue<EffectBullet>();
    private bool ifNormal = false;
    private bool initialized = false;
    

    protected override void Awake()
    {
        base.Awake();
        InitBulletSystem();
       
    }

    private void OnEnable()
    {
        if (initialized)
            return;
        initialized = true;
        foreach (var bulletList in bulletListsDic)
        {
            VBattleRootEventCenter.Instance.RegisterListener(bulletList.Key, (dict) =>
            {
                AddEffectBullet(bulletList.Key);
            });
        }
    }

    private void Update()
    {
        if (!ifGenerate)
        {
            return;
        }
        generateTimer += Time.deltaTime;
        if (generateTimer >= timeBetweenBullet)
        {
            OnBulletGenerate(GenerateBullet());
            timeBetweenBullet=Random.Range(timeMin, timeMax);
            generateTimer = 0;
        }
    }

    private BulletInfo GenerateBullet()
    {
        BulletInfo bi = new BulletInfo();
        
        if (effectBulletQueue.Count > 0)
        {
            ifNormal = false;
            if (effectBulletCount > 0)
            {
                bi=bulletGenerator.GenerateBullet();
                effectBulletCount--;
            }
            else
            {
                EffectBullet effectBullet = effectBulletQueue.Dequeue();
                effectBulletCount=effectBullet.bulletCount;
                bulletGenerator.LoadBullet(bulletListsDic[effectBullet.bulletState]);
                bi=bulletGenerator.GenerateBullet();
                effectBulletCount--;
            }
        }
        else
        {
            if (effectBulletCount > 0)
            {
                bi=bulletGenerator.GenerateBullet();
                effectBulletCount--;
                
            }
            else
            {
                if (!ifNormal)
                {
                    bulletGenerator.LoadBullet(bulletListsDic[VBattleEventKey.Default]);
                    ifNormal=true;
                }
                bi=bulletGenerator.GenerateBullet();
            }
        }

        return bi;
    }

    public void ResetSystem()
    {
        effectBulletCount = 0;
        generateTimer = 0;
        effectBulletQueue.Clear();
        ifNormal=false;
    }

    public void AddEffectBullet(VBattleEventKey newState, int effectBulletNum = 10)
    {
        EffectBullet effectBullet = new EffectBullet();
        effectBullet.bulletState = newState;
        effectBullet.bulletCount = effectBulletNum;
        effectBulletQueue.Enqueue(effectBullet);
    }

    protected virtual void OnBulletGenerate(BulletInfo arg0)
    {
        onBulletGenerate?.Invoke(arg0);
    }
}

public class BulletGenerator
{
    private BulletInfo[] bullets;
    private int[] indices;
    private int currentPosition;
    private int remainingCount;
    
    public void LoadBullet(BulletList sourceList)
    {
        bullets = new BulletInfo[sourceList.bullets.Count];
        indices = new int[sourceList.bullets.Count];
        
        for (int i = 0; i < sourceList.bullets.Count; i++)
        {
            bullets[i] = sourceList.bullets[i];
            indices[i] = i;
        }
        
        Reset();
    }
    public void Reset()
    {
        // 重置索引数组
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = i;
        }
        
        // Fisher-Yates洗牌索引
        for (int i = indices.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = indices[i];
            indices[i] = indices[j];
            indices[j] = temp;
        }
        
        currentPosition = 0;
        remainingCount = indices.Length;
    }
    
    public BulletInfo GenerateBullet()
    {
        if (remainingCount == 0)
        {
            Reset();
        }

        BulletInfo bullet = bullets[indices[currentPosition]];
        currentPosition++;
        remainingCount--;
        
        return bullet;
    }

    public int RemainingCount => remainingCount;
    public bool IsRoundComplete => remainingCount == 0;
}
