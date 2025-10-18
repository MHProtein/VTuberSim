using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BulletData
{
    public string content;           // 弹幕内容
    public Color color = Color.black; // 弹幕颜色
    public int fontSize = 30;        // 字体大小
    public float speed = 100f;       // 滚动速度
    public bool isPriority = false;  // 是否优先显示
}
public class BulletItem : MonoBehaviour
{
    public Text textComponent;
    public RectTransform rectTransform;
    public ContentSizeFitter sizeFitter;
    
    private BulletData bulletData;
    private float startX;
    private float endX;
    private bool isActive = false;
    
    public Action<BulletItem> onRecycle;

    private void Awake()
    {
        
    }

    public void Initialize(BulletData data, float containerWidth, float yPosition)
    {
        bulletData = data;
        
        // 设置文本内容
        textComponent.text = data.content;
        textComponent.color = data.color;
        textComponent.fontSize = data.fontSize;
        
        // 强制立即布局更新
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        // 设置位置
        float textWidth = rectTransform.rect.width;
        rectTransform.anchoredPosition = new Vector2(containerWidth, yPosition);
        
        // 计算滚动参数
        startX = containerWidth;
        endX = -textWidth;
        
        isActive = true;
        gameObject.SetActive(true);
    }
    
    private void Update()
    {
        if (!isActive) return;
        
        // 水平滚动
        float newX = rectTransform.anchoredPosition.x - bulletData.speed * Time.deltaTime;
        rectTransform.anchoredPosition = new Vector2(newX, rectTransform.anchoredPosition.y);
        
        // 检查是否超出屏幕
        if (newX < endX)
        {
            Recycle();
        }
    }
    
    private void Recycle()
    {
        isActive = false;
        gameObject.SetActive(false);
        RollBulletPanel.Instance.currentBulletCount--;
        onRecycle?.Invoke(this);
    }
    
    public bool IsActive => isActive;
    public float CurrentX => rectTransform.anchoredPosition.x;
    public float Width => textComponent.preferredWidth;
}
