using System.Collections.Generic;
using UnityEngine;

namespace System
{
    public static class ObserverSystem
    {
        private static readonly Dictionary<Type, Delegate> _listeners = new();

        public static void Subscribe<T>(Action<T> callback)
        {
            if (callback == null)
            {
                Debug.LogError("ObserverSystem: Attempted to add a null listener.");
                return;
            }

            Type eventType = typeof(T);
            _listeners.TryGetValue(eventType, out Delegate currentListeners);
            _listeners[eventType] = Delegate.Combine(currentListeners, callback);
        }

        public static void UnSubscribe<T>(Action<T> callback)
        {
            if (callback == null)
            {
                return;
            }

            Type eventType = typeof(T);
            if (!_listeners.TryGetValue(eventType, out Delegate currentListeners))
            {
                return;
            }

            Delegate remainingListeners = Delegate.Remove(currentListeners, callback);
            if (remainingListeners == null)
            {
                _listeners.Remove(eventType);
                return;
            }

            _listeners[eventType] = remainingListeners;
        }

        public static void Announce<T>(T eventData)
        {
            Type eventType = typeof(T);
            if (!_listeners.TryGetValue(eventType, out Delegate listeners))
            {
                return;
            }

            foreach (Delegate listener in listeners.GetInvocationList())
            {
                try
                {
                    ((Action<T>)listener).Invoke(eventData);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        public static void ClearAllSubscribeMessages()
        {
            _listeners.Clear();
        }
    }
}
