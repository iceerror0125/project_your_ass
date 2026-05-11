using System.Collections.Generic;
using ProtectYourAss.System;
using UnityEngine;

namespace System
{
    /// <summary>
    /// Simple and robust Observer Pattern system using Enum and Action<object>.
    /// It supports passing optional parameters and is safe for unregistration during event broadcasting.
    /// </summary>
    public static class ObserverSystem
    {
        // Internal storage for listeners.
        private static readonly Dictionary<ObserveMessage, Action<object>> _listeners = new Dictionary<ObserveMessage, Action<object>>();

        /// <summary>
        /// Registers a callback for a specific event.
        /// </summary>
        /// <param name="eventID">The event ID enum.</param>
        /// <param name="callback">The callback method (must accept an object parameter).</param>
        public static void AddListener(ObserveMessage eventID, Action<object> callback)
        {
            if (callback == null)
            {
                Debug.LogError("ObserverSystem: Attempted to add a null listener.");
                return;
            }

            // If the key exists, add the delegate. If not, create a new entry.
            if (_listeners.ContainsKey(eventID))
            {
                _listeners[eventID] += callback;
            }
            else
            {
                _listeners.Add(eventID, callback);
            }
        }

        /// <summary>
        /// Unregisters a callback for a specific event.
        /// </summary>
        /// <param name="eventID">The event ID enum.</param>
        /// <param name="callback">The callback method to remove.</param>
        public static void RemoveListener(ObserveMessage eventID, Action<object> callback)
        {
            if (_listeners.ContainsKey(eventID))
            {
                _listeners[eventID] -= callback;

                // Cleanup dictionary if no listeners left for this event ID.
                if (_listeners[eventID] == null)
                {
                    _listeners.Remove(eventID);
                }
            }
        }

        /// <summary>
        /// Posts (triggers) an event with optional parameter data.
        /// </summary>
        /// <param name="eventID">The event ID enum.</param>
        /// <param name="data">Optional data to pass to listeners. Can be null.</param>
        public static void PostEvent(ObserveMessage eventID, object data = null)
        {
            if (_listeners.TryGetValue(eventID, out var callback))
            {
                // Invoke all registered listeners for this event.
                // We use Try-Catch inside to ensure one listener's failure doesn't stop everyone else.
                var invocationList = callback.GetInvocationList();
                foreach (var del in invocationList)
                {
                    try
                    {
                        (del as Action<object>)?.Invoke(data);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"ObserverSystem: Error invoking {eventID} on {del.Method.Name}: {ex.Message}\n{ex.StackTrace}");
                    }
                }
            }
        }

        /// <summary>
        /// (Optional Audit Helper) Clears all listeners. Useful for testing or full scene reload.
        /// </summary>
        public static void ClearAllListeners()
        {
            _listeners.Clear();
            Debug.Log("ObserverSystem: All listeners cleared.");
        }
    }
}
