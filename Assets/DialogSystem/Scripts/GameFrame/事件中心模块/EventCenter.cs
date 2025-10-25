using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EventInfoBase{}

public class EventInfo<T> : EventInfoBase
{
    private UnityAction<T> actions=null;

    public void AddListener(UnityAction<T> action)
    {
        actions += action;
    }

    public void RemoveListener(UnityAction<T> action)
    {
        actions -= action;
    }

    public void Invoke(T obj)
    {
        actions?.Invoke(obj);   
    }
}
public class EventInfo : EventInfoBase
{
    private UnityAction actions=null;

    public void AddListener(UnityAction action)
    {
        actions += action;
    }

    public void RemoveListener(UnityAction action)
    {
        actions -= action;
    }

    public void Invoke()
    {
        actions?.Invoke();   
    }
}
public class EventCenter : BaseManager<EventCenter>
{
    private EventCenter(){}
    
    //用于 记录 事件关联的 对应的逻辑
    private Dictionary<E_EventType,EventInfoBase> eventDic = new Dictionary<E_EventType, EventInfoBase>();

    public void EventTrigger<T>(E_EventType eventType,T info)
    {
        //有过订阅记录的事件，才会去处理逻辑
        if (eventDic.ContainsKey(eventType))
        {
            //去执行对应的逻辑
            (eventDic[eventType] as EventInfo<T>)?.Invoke(info);
        }
    }
    public void EventTrigger(E_EventType eventType)
    {
        //有过订阅记录的事件，才会去处理逻辑
        if (eventDic.ContainsKey(eventType))
        {
            //去执行对应的逻辑
            (eventDic[eventType] as EventInfo)?.Invoke();
        }
    }

    //订阅事件
    public void AddEventListener<T>(E_EventType eventType, UnityAction<T> func)
    {
        if (eventDic.ContainsKey(eventType))
        {
            (eventDic[eventType] as EventInfo<T>)?.AddListener(func);
        }
        else
        {
            eventDic.Add(eventType, new EventInfo<T>());
            (eventDic[eventType] as EventInfo<T>)?.AddListener(func);
        }
    }
    public void AddEventListener (E_EventType eventType, UnityAction func)
    {
        if (eventDic.ContainsKey(eventType))
        {
            (eventDic[eventType] as EventInfo)?.AddListener(func);
        }
        else
        {
            eventDic.Add(eventType, new EventInfo());
            (eventDic[eventType] as EventInfo)?.AddListener(func);
        }
    }

    //取消订阅事件
    public void RemoveEventListener<T>(E_EventType eventType, UnityAction<T> func)
    {
        if (eventDic.ContainsKey(eventType))
        {
            (eventDic[eventType] as EventInfo<T>)?.RemoveListener(func); 
        }
    }

    public void RemoveEventListener(E_EventType eventType, UnityAction func)
    {
        if (eventDic.ContainsKey(eventType))
        {
            (eventDic[eventType] as EventInfo)?.RemoveListener(func); 
        }
    }
    //清除所有事件的监听
    public void Clear()
    {
        eventDic.Clear();
    }

    
    //清除特定事件的监听
    public void Clear(E_EventType eventType)
    {
        eventDic.Remove(eventType);
    }


}
