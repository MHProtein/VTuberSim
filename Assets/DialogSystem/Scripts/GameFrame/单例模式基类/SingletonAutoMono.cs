using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonAutoMono <T>: MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                //动态创建 动态挂载
                //在场景上创建空物体
                GameObject obj = new GameObject();
                //得到T脚本的类名 为对象改名 这样在编辑器中可以明确地看到
                //单例模式脚本依附的GameObject
                obj.name = typeof(T).Name;
                //动态挂载 对应的 单例模式脚本
                instance = obj.AddComponent<T>();
                //过场景时不移除对象 保证他在整个生命周期中都存在
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }
 
   
}
