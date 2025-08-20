using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

/// <summary>
/// 资源信息基类 主要用于里氏替换原则 父类容器装子类对象
/// </summary>
public abstract class ResInfoBase
{
    //引用计数
    public int refCount;
}

/// <summary>
/// 资源信息对象 主要用于存储资源信息 异步加载委托信息 异步加载 协程信息
/// </summary>
/// <typeparam name="T">资源类型</typeparam>
public class ResInfo<T> : ResInfoBase
{
    //资源
    public T asset;
    //主要用于异步加载结束后 传递资源到外部的委托
    public UnityAction<T> callback;
    //用于存储异步加载时 开启的协同程序函数对象
    public Coroutine coroutine;
    //是否马上删除标识
    public bool isDel = false;
    
    public void AddRefCount(){refCount++;}

    public void SubRefCount()
    {
        refCount--;
        if (refCount < 0)
        {
            Debug.LogError("引用计数小于0了，请检查使用和卸载是否配对执行");
        }
    }
}

public class ResMgr :BaseManager<ResMgr>
{
    //用于存储加载过的资源或者加载中的资源容器
    private Dictionary<string, ResInfoBase> resInfoDic = new Dictionary<string, ResInfoBase>();
    
    private ResMgr()
    {
    }

    
    /// <summary>
    /// 同步加载资源的方法
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <typeparam name="T">资源类型</typeparam>
    /// <returns></returns>
    public T Load<T>(string path) where T : UnityEngine.Object
    {
        string resName=path+"_"+typeof(T).Name;
        ResInfo<T> resInfo;
        if (!resInfoDic.ContainsKey(resName))
        {
            //直接同步加载 并且记录资源信息 到字典中 方便下次直接取出来用
            T res = Resources.Load<T>(resName);
            resInfo = new ResInfo<T>();
            resInfo.asset = res;
            resInfo.AddRefCount();
            resInfoDic.Add(resName, resInfo);
            return res;
        }
        else
        {
            //取出字典中的记录
            resInfo = resInfoDic[resName] as ResInfo<T>;
            resInfo.AddRefCount();
            //存在异步加载 还在加载中
            if (resInfo.asset == null)
            {
                //停止异步加载
                MonoMgr.Instance.StopCoroutine(resInfo.coroutine);
                //直接采用同步的方式加载成功
                T res = Resources.Load<T>(resName);
                //记录加载成功的资源
                resInfo.asset = res;
                //执行那些等待异步加载完成的委托
                resInfo.callback?.Invoke(res);
                //回调结束 异步加载也停了 清除无用的引用
                resInfo.callback = null;
                resInfo.coroutine = null;
                //返回给外部使用
                return res;
            }
        }

        return Resources.Load<T>(path);
    }

    /// <summary>
    /// 异步加载资源的方法
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="callback">加载结束后的回调函数 当异步加载资源结束后才会调用</param>
    /// <typeparam name="T">资源类型</typeparam>
    public void LoadAsync<T>(string path, UnityAction<T> callback) where T:UnityEngine.Object
    {
        //资源的唯一ID，是通过 路径名_资源类型 拼接而成
        string resName=path+"_"+typeof(T).Name;
        ResInfo<T> resInfo;
        if (!resInfoDic.ContainsKey(resName))
        {
            //声明一个 资源信息对象
            resInfo = new ResInfo<T>();
            resInfo.AddRefCount();
            //将资源记录添加到字典中（资源还没有加载成功）
            resInfoDic.Add(resName, resInfo);
            //记录传入的委托函数 一会儿加载完成了 再使用
            resInfo.callback += callback;
            //开启协程去 异步加载 并且记录协同程序（用于之后可能的停止）
            resInfo.coroutine=MonoMgr.Instance.StartCoroutine(ReallyLoadAsync<T>(path));
        }
        else
        {
            resInfo = resInfoDic[resName] as ResInfo<T>;
            resInfo.AddRefCount();                                           
            if (resInfo.asset == null)
            {
                resInfo.callback+=callback;
            }
            else
            {
                callback?.Invoke(resInfo.asset);
            }
        }
        

    }

    private IEnumerator ReallyLoadAsync<T>(string path) where T : UnityEngine.Object
    {
        //异步加载资源
        ResourceRequest rq = Resources.LoadAsync<T>(path);
        //等待资源加载结束后 才会继续执行yield return后面的代码
        yield return rq;
        string resName=path+"_"+typeof(T).Name;
        if (resInfoDic.ContainsKey(resName))
        {
            
            ResInfo<T> resInfo = resInfoDic[resName] as ResInfo<T>;
            //取出资源信息 并且记录加载完成的资源
            resInfo.asset=rq.asset as T;
            if (resInfo.refCount == 0)
            {
                UnloadAsset<T>(path,resInfo.isDel,null,false);
            }
            else
            {
                //将加载完成的资源传递出去
                resInfo.callback?.Invoke(resInfo.asset);
                //加载完毕后 这些引用就可以清空 避免引用的占用 可能带来的潜在的内存泄漏问题
                resInfo.callback = null;
                resInfo.coroutine = null;
            }
            
        }
        
    }
    
    /// <summary>
    /// 异步加载资源的方法
    /// </summary>
    /// <param name="path">资源路径</param>
    /// /// <param name="type">加载资源类型</param>
    /// <param name="callback">加载结束后的回调函数 当异步加载资源结束后才会调用</param>
    [Obsolete("注意：建议使用泛型加载方式，如果实在要用Type加载，一定不能和泛型加载混用去加载同类型同名资源")]
    public void LoadAsync(string path,Type type, UnityAction<UnityEngine.Object> callback) 
    {
        //资源的唯一ID，是通过 路径名_资源类型 拼接而成
        string resName=path+"_"+type.Name;
        ResInfo<UnityEngine.Object> resInfo;
        if (!resInfoDic.ContainsKey(resName))
        {
            //声明一个 资源信息对象
            resInfo = new ResInfo<UnityEngine.Object>();
            resInfo.AddRefCount();
            //将资源记录添加到字典中（资源还没有加载成功）
            resInfoDic.Add(resName, resInfo);
            //记录传入的委托函数 一会儿加载完成了 再使用
            resInfo.callback += callback;
            //开启协程去 异步加载 并且记录协同程序（用于之后可能的停止）
            resInfo.coroutine=MonoMgr.Instance.StartCoroutine(ReallyLoadAsync<UnityEngine.Object>(path));
        }
        else
        {
            resInfo = resInfoDic[resName] as ResInfo<UnityEngine.Object>;
            resInfo.AddRefCount();
            if (resInfo.asset == null)
            {
                resInfo.callback+=callback;
            }
            else
            {
                callback?.Invoke(resInfo.asset);
            }
        }
    }

    private IEnumerator ReallyLoadAsync(string path,Type type)
    {
        //异步加载资源
        ResourceRequest rq = Resources.LoadAsync<UnityEngine.Object>(path);
        //等待资源加载结束后 才会继续执行yield return后面的代码
        yield return rq;
        string resName=path+"_"+type.Name;
        if (resInfoDic.ContainsKey(resName))
        {
            
            ResInfo<UnityEngine.Object> resInfo = resInfoDic[resName] as ResInfo<UnityEngine.Object>;
            //取出资源信息 并且记录加载完成的资源
            resInfo.asset=rq.asset;
            if (resInfo.refCount == 0) 
            {
                UnloadAsset(path,type,resInfo.isDel,null,false);
            }
            else
            {
                //将加载完成的资源传递出去
                resInfo.callback?.Invoke(resInfo.asset);
                //加载完毕后 这些引用就可以清空 避免引用的占用 可能带来的潜在的内存泄漏问题
                resInfo.callback = null;
                resInfo.coroutine = null;
            }
           
        }

    }

    /// <summary>
    /// 指定卸载一个资源
    /// </summary
    /// <param name="path">卸载资源的文件路径</param>
    /// <param name="isDel">引用计数为零时是否马上移除资源的bool标识</param>
    /// <param name="callback">异步加载未完成时尚未使用的回调函数</param>
    public void UnloadAsset<T>(string path,bool isDel=false,UnityAction<T> callback=null,bool isSub=true) where T : UnityEngine.Object
    {
        string resName=path+"_"+typeof(T).Name;
        //判断是否存在对应资源
        if (resInfoDic.ContainsKey(resName))
        {
            ResInfo<T> resInfo = resInfoDic[resName] as ResInfo<T>;
            resInfo.isDel=isDel;
            if(isSub)
                resInfo.SubRefCount();
            if (resInfo.asset&& resInfo.refCount==0&&isDel)
            {
                resInfoDic.Remove(resName);
                Resources.UnloadAsset(resInfo.asset as UnityEngine.Object);
            }
            else if(!resInfo.asset)
            {
                //为了保险起见 一定要让资源移除了
                //改变标识 待删除
                if(resInfo.callback!=null)
                    resInfo.callback-=callback;
            }
        }
    }

    /// <summary>
    /// 指定卸载一个资源
    /// </summary>
    /// <param name="path">卸载资源的文件路径</param>
    /// <param name="type">卸载资源的类型</param>
    /// <param name="isDel">引用计数为零时是否马上移除资源的bool标识</param>
    public void UnloadAsset(string path,Type type,bool isDel=false,UnityAction<UnityEngine.Object> callback=null,bool isSub=true) 
    {
        string resName=path+"_"+type.Name;
        //判断是否存在对应资源
        if (resInfoDic.ContainsKey(resName))
        {
            ResInfo<UnityEngine.Object> resInfo = resInfoDic[resName] as ResInfo<UnityEngine.Object>;
            resInfo.isDel=isDel;
            if(isSub)
                resInfo.SubRefCount();
            if (resInfo.asset&& resInfo.refCount==0&&isDel)
            {
                resInfoDic.Remove(resName);
                Resources.UnloadAsset(resInfo.asset);
            }
            else if(resInfo.asset==null)
            {
                //为了保险起见 一定要让资源移除了
                //改变标识 待删除
                resInfo.callback-=callback;
            }
        }
    }
    /// <summary>
    /// 异步卸载对应没有使用的Resources相关资源
    /// </summary>
    /// <param name="callback">回调函数</param>
    public void UnloadUnusedAssets(UnityAction callback)
    {
        MonoMgr.Instance.StartCoroutine(ReallyUnloadUnusedAssets(callback));
    }

    private IEnumerator ReallyUnloadUnusedAssets(UnityAction callback)
    {
        //就是在真正移除不使用的资源之前 应该把我们自己记录的那些引用计数为0 并且没有被移除记录的资源
        //移除掉
        List<string> list = new List<string>();
        foreach (var resId in resInfoDic.Keys)
        {
            if (resInfoDic[resId].refCount == 0)
            {
                list.Add(resId);
            }
        }

        foreach (var resId in list)
        {
            resInfoDic.Remove(resId);
        }

        AsyncOperation ao = Resources.UnloadUnusedAssets();
        yield return ao;
        //卸载完毕后通知外部
        callback();
    }

    public void ClearDic(UnityAction callback)
    {
        MonoMgr.Instance.StartCoroutine(ReallyClearDic(callback));
    }

    private IEnumerator ReallyClearDic(UnityAction callback)
    {
        resInfoDic.Clear();
        AsyncOperation ao = Resources.UnloadUnusedAssets();
        yield return ao;
        callback();
    }
}
