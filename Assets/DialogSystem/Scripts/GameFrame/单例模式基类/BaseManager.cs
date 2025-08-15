using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class BaseManager<T> where T : class//,new()
{
    private static T instance;

    protected static readonly object locker = new object();
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (locker)
                    if (instance == null)
                    {
                        //instance = new T();
                        //利用反射来获得无参私有的构造函数 用来对象的实例化
                        Type type = typeof(T);  
                        ConstructorInfo info=type.GetConstructor(
                            BindingFlags.Instance | BindingFlags.NonPublic, 
                            null, 
                            Type.EmptyTypes, 
                            null);
                        if(info!=null)
                            instance = info.Invoke(null) as T;
                        else
                            Debug.LogError("没有找到对应的无参构造函数");
                    }
            }
            return instance;
        }
    }

    // public static T GetInstance()
    // {
    //     return instance ??= new T();
    // }
}
