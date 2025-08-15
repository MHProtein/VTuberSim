using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolData
{
    //记录未被使用的对象
    private Stack<GameObject> dataStack=new Stack<GameObject>();
    //记录正在使用中的对象
    private List<GameObject> usedData=new List<GameObject>();
    //抽屉的根对象
    private GameObject rootObj;
    //抽屉上限，场上同时存在的数量上限
    private int maxNum;
    public int Count=>dataStack.Count;
    
    public int UsedCount => usedData.Count;
    
    public bool NeedCreate=>UsedCount<maxNum;

    /// <summary>
    /// 初始化构造函数
    /// </summary>
    /// <param name="root">柜子（缓存池）父对象</param>
    /// <param name="name">抽屉父对象的名字</param>
    /// <param name="usedObj">创建抽屉时记录的被使用的对象</param>
    public PoolData(GameObject root,string name,GameObject usedObj)
    {
        if (PoolMgr.ifOpenLayout)
        {
            rootObj = new GameObject(name);
            rootObj.transform.SetParent(root.transform);
        }
        usedData.Add(usedObj);
        
        PoolObj poolObj=usedObj.GetComponent<PoolObj>();
        if (!poolObj)
        {
            Debug.LogError("请为使用缓存池功能的预设体对象挂载PoolObj脚本 用于设置数量上限");
            return;
        }
        //记录上限数量值
        maxNum=poolObj.maxNum;
    }

    public GameObject Pop()
    {
        GameObject obj;
        if (dataStack.Count > 0)
        {
            obj = dataStack.Pop();
            usedData.Add(obj);
        }
        else
        {
            //选取使用时间最长的对象
            obj = usedData[0];
            //将对象取出重新置于List末尾
            usedData.RemoveAt(0);
            usedData.Add(obj);  
        }

        //激活对象
        obj.SetActive(true);
        //断开父子关系
        if (PoolMgr.ifOpenLayout)
        obj.transform.SetParent(null);
        
        return obj;
    }

    public void Push(GameObject obj)
    {
        
        obj.SetActive(false);
        if (PoolMgr.ifOpenLayout)
        obj.transform.SetParent(rootObj.transform);
        //通过栈记录对应的对象数据
        dataStack.Push(obj);
        //对象不再使用了，从使用中的列表移除
        usedData.Remove(obj);
    }

    /// <summary>
    /// 记录使用中的对象
    /// </summary>
    /// <param name="obj"></param>
    public void PushUseList(GameObject obj)
    {
        usedData.Add(obj);
    }
}

public class PoolMgr : BaseManager<PoolMgr>
{
    private PoolMgr() { }
    
    //池子根对象
    private GameObject poolObj;
    
    //是否开启对象池自动布局的功能（优化实际运行的性能开销）
    public static bool ifOpenLayout=true;
    
   //柜子当中有抽屉的表现
    private Dictionary<string, PoolData > poolDic = new Dictionary<string, PoolData>();

    public GameObject GetObj(string poolName)
    {
        
        if (ifOpenLayout&&!poolObj)
        {
            poolObj=new GameObject("Pool");
        }
        
        GameObject obj;

        #region 加入了 数量上限 后的逻辑

        //没有抽屉时 或者 有抽屉，但是抽屉里没有有用的对象并且使用中的对象没有超过上限时
        if (!poolDic.ContainsKey(poolName) ||
            (poolDic.ContainsKey(poolName) && poolDic[poolName].Count==0&&poolDic[poolName].NeedCreate))
        {
            //=通过资源加载去实例化一个GameObject
            obj = GameObject.Instantiate(Resources.Load<GameObject>(poolName));
            //实例化对象默认会在名字后加（clone）
            //为了方便往里面放（抽屉的索引和所放对象的名称一致），所以重命名
            obj.name = poolName;
            //如果没有抽屉，创建一个抽屉，并记录使用的物体（在构造函数中记录）
            if (!poolDic.ContainsKey(poolName))
                poolDic.Add(poolName, new PoolData(poolObj, poolName, obj));
            else //记录使用的物体
                poolDic[poolName].PushUseList(obj);
        }
        //如果有抽屉，并且抽屉里有没用的对象或者使用中的对象超过上限时
        else 
        {
            obj=poolDic[poolName].Pop();
        }
        

        #endregion
        
        #region 没有加入上限时 的逻辑
        
        // //有抽屉 且抽屉里面有对象 才会去拿
        // if (poolDic.ContainsKey(poolName) && poolDic[poolName].Count > 0)
        // {
        //     obj=poolDic[poolName].Pop();
        // }
        // //否则就应该去创造
        // else
        // {
        //     //没有的时候 通过资源加载去实例化一个GameObject
        //     obj=GameObject.Instantiate(Resources.Load<GameObject>(poolName));
        //     obj.name = poolName;
        // }
        
        #endregion
       

        return obj;
    }

    /// <summary>
    /// 往缓存池中放入对象
    /// </summary>
    /// <param name="poolName"></param>
    /// <param name="obj"></param>
    public void PushObj( GameObject obj)
    {
      
        
        //如果存在抽屉，就直接放
        poolDic[obj.name].Push(obj);
        
        // else
        // {
        //     //如果不存在，就创建抽屉，再放
        //     poolDic.Add(obj.name, new PoolData(poolObj,obj.name));
        //     poolDic[obj.name].Push(obj);
        // }
    }

    /// <summary>
    /// 用于清除整个柜子中的数据
    /// 使用场景 主要是 切场景时
    /// </summary>
    public void ClearPool()
    {
        poolDic.Clear();
        poolObj=null;
    }
}
