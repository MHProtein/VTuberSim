using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrackManager : MonoBehaviour
{
    [SerializeField] private RectTransform container;
    [SerializeField] private GameObject BulletItemPrefab;
    
    private List<BulletTrack> tracks = new List<BulletTrack>();
    private Queue<BulletItem> itemPool = new Queue<BulletItem>();
    private List<BulletItem> activeItems = new List<BulletItem>();
    
    [Header("轨道设置")]
    [SerializeField] private int trackCount = 8;
    [SerializeField] private float trackHeight = 40f;
    [SerializeField] private float minSpacing = 20f;
    
    public void Initialize()
    {
        CreateTracks();
        PrewarmPool(40);
    }
    
    private void CreateTracks()
    {
        float containerHeight = container.rect.height;
        float trackSpacing = containerHeight / trackCount;
        
        for (int i = 0; i < trackCount; i++)
        {
            float yPos = containerHeight / 2 - (i + 0.5f) * trackSpacing;
            tracks.Add(new BulletTrack(i, yPos, trackSpacing));
        }
    }
    
    private void PrewarmPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            CreatePoolItem();
        }
    }
    
    private BulletItem CreatePoolItem()
    {
        GameObject itemObj = Instantiate(BulletItemPrefab, container);
        BulletItem item = itemObj.GetComponent<BulletItem>();
        item.onRecycle = OnItemRecycle;
        item.gameObject.SetActive(false);
        itemPool.Enqueue(item);
        return item;
    }
    
    public bool TrySpawnBullet(BulletData data)
    {
        // 查找合适的轨道
        BulletTrack suitableTrack = FindSuitableTrack(data);
        if (suitableTrack == null) return false;
        
        // 从对象池获取弹幕项
        BulletItem item = GetPoolItem();
        if (item == null) return false;
        
        // 初始化弹幕
        item.Initialize(data, container.rect.width, suitableTrack.YPosition);
        suitableTrack.Occupy(item);
        activeItems.Add(item);
        
        return true;
    }
    
    private BulletTrack FindSuitableTrack(BulletData data)
    {
        // // 优先尝试空闲轨道
        // foreach (var track in tracks)
        // {
        //     if (track.IsAvailable)
        //     {
        //         return track;
        //     }
        // }
        
        // 如果没有空闲轨道，寻找可以插入的轨道
        foreach (var track in tracks)
        {
            if (track.CanAcceptNewBullet(data, minSpacing))
            {
                return track;
            }
        }
        
        return null;
    }
    
    private BulletItem GetPoolItem()
    {
        if (itemPool.Count > 0)
        {
            return itemPool.Dequeue();
        }
        
        // 池中没有可用对象，创建新的
        return CreatePoolItem();
    }
    
    private void OnItemRecycle(BulletItem item)
    {
        // 从活跃列表中移除
        activeItems.Remove(item);
        
        // 通知轨道释放
        foreach (var track in tracks)
        {
            if (track.Contains(item))
            {
                track.Release(item);
                break;
            }
        }
        
        // 回收到对象池
        itemPool.Enqueue(item);
    }
    
    public void ClearAllBullet()
    {
        foreach (var item in activeItems)
        {
            item.gameObject.SetActive(false);
            itemPool.Enqueue(item);
        }
        activeItems.Clear();
        
        foreach (var track in tracks)
        {
            track.Clear();
        }
    }
}
public class BulletTrack
{
    public int TrackIndex { get; private set; }
    public float YPosition { get; private set; }
    public float Height { get; private set; }
    public bool IsAvailable => occupiedItems.Count == 0;
    
    private List<BulletItem> occupiedItems = new List<BulletItem>();
    
    public BulletTrack(int index, float yPos, float height)
    {
        TrackIndex = index;
        YPosition = yPos;
        Height = height;
    }
    
    public void Occupy(BulletItem item)
    {
        if (!occupiedItems.Contains(item))
        {
            occupiedItems.Add(item);
        }
    }
    
    public void Release(BulletItem item)
    {
        occupiedItems.Remove(item);
    }
    
    public bool Contains(BulletItem item)
    {
        return occupiedItems.Contains(item);
    }
    
    public bool CanAcceptNewBullet(BulletData data, float minSpacing)
    {
        if (occupiedItems.Count == 0) return true;
        
        // 获取最后一个弹幕的位置信息
        BulletItem lastItem = occupiedItems[occupiedItems.Count - 1];
        
        // 检查间距是否足够
        float lastItemRightEdge = lastItem.CurrentX + lastItem.Width;
        float requiredSpacing = minSpacing + lastItem.Width * 0.5f;
        
        return lastItemRightEdge < requiredSpacing;
    }
    
    public void Clear()
    {
        occupiedItems.Clear();
    }
}