using System;
using System.Collections.Generic;

namespace VTuber.Core.EventCenter
{
    public class VEventCenter<InstanceType, KeyType, DelegateType>
        where DelegateType : Delegate where InstanceType : VEventCenter<InstanceType, KeyType, DelegateType>
    {
        private static InstanceType _instance;

        public static InstanceType Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Activator.CreateInstance<InstanceType>();
                    _instance.Init();
                }

                return _instance;
            }
        }

        public Dictionary<KeyType, DelegateType> Events { get; private set; }

        public virtual void Init()
        {
            Events = new Dictionary<KeyType, DelegateType>();
        }

        public virtual bool Raise(KeyType key, params object[] args)
        {
            if (Events.TryGetValue(key, out var _delegate))
            {
                if (_delegate == null)
                    //VDebug.LogWarning($"Event with key {key} has no listeners.");
                    return false;
                _delegate.DynamicInvoke(args);
                return true;
            }

            return false;
        }

        public virtual void RegisterListener(KeyType key, DelegateType @delegate)
        {
            if (Events.TryGetValue(key, out var outDelegate))
                Events[key] = (DelegateType)Delegate.Combine(outDelegate, @delegate);
            else
                Events.Add(key, @delegate);
        }

        public virtual bool RemoveListener(KeyType key, DelegateType @delegate)
        {
            if (Events.TryGetValue(key, out var outDelegate))
            {
                Events[key] = (DelegateType)Delegate.Remove(outDelegate, @delegate);
                return true;
            }

            return false;
        }

        public virtual bool EventExists(KeyType key, DelegateType @delegate)
        {
            if (Events.TryGetValue(key, out var outDelegate))
            {
                var delegates = outDelegate.GetInvocationList();
                foreach (var dele in delegates)
                    if (dele.Equals(@delegate))
                        return true;
            }

            return false;
        }

        public virtual void Clear()
        {
            Events.Clear();
        }
    }
}