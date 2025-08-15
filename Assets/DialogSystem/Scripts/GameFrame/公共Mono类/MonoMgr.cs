using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoMgr :SingletonAutoMono<MonoMgr>
{
    private event UnityAction updateEvent;
    private event UnityAction fixedUpdateEvent;
    private event UnityAction lateUpdateEvent;

    public void AddUpdateListener(UnityAction unityAction)
    {
        updateEvent += unityAction;
    }
    public void AddFixedUpdateListener(UnityAction unityAction)
    {
        fixedUpdateEvent += unityAction;
    }
    public void AddLateUpdateListener(UnityAction unityAction)
    {
        lateUpdateEvent += unityAction;
    }
    public void RemoveUpdateListener(UnityAction unityAction)
    {
        updateEvent -= unityAction;
    }
    public void RemoveFixedUpdateListener(UnityAction unityAction)
    {
        fixedUpdateEvent -= unityAction;
    }
    public void RemoveLateUpdateListener(UnityAction unityAction)
    {
        lateUpdateEvent -= unityAction;
    }

    private void Update()
    {
        updateEvent?.Invoke();
    }

    private void FixedUpdate()
    {
       fixedUpdateEvent?.Invoke();
    }

    private void LateUpdate()
    {
        lateUpdateEvent?.Invoke();
    }
}
