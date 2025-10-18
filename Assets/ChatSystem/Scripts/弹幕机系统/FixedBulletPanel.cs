using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FixedBulletPanel : SingletonMono<FixedBulletPanel>
{
    public Transform bulletRoot;
    public GameObject bulletPrefab;
    public ScrollRect scrollRect;
    public RectTransform scrollContent;
    public float scrollDelay = 0.1f;
    
    protected override void Awake()
    {
        base.Awake();
        BulletSystem.Instance.onBulletGenerate+=OnBulletGenerate;
    }
   

    private void Start()
    {
        
    }

    public void ShowMe()
    {
        this.gameObject.SetActive(true);
    }

    public void HideMe()
    {
        this.gameObject.SetActive(false);
        StartCoroutine(Clear());
    }

    public void OnBulletGenerate(BulletInfo bulletInfo)
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletRoot);
        Canvas.ForceUpdateCanvases();
        bullet.GetComponent<BulletObj>().senderName.text = bulletInfo.senderName;
        bullet.GetComponent<BulletObj>().bulletContent.text = bulletInfo.bulletContent;
        
        AdjustScrollView();
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

    public IEnumerator Clear()
    {
        while (bulletRoot.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        yield return null;
    }
}
