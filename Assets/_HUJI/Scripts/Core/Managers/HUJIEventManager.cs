using System;
using System.Collections.Generic;

namespace HUJI
{
    public class HUJIEventManager
    {
        private bool _isInvoking;

        private readonly List<(HUJIEventName eventName, Action<object> listener, bool isAddition)> _pendingChanges = new();
        private readonly Dictionary<HUJIEventName, Action<object>> _activeListeners = new();

        public void AddListener(HUJIEventName eventName, Action<object> listener)
        {
            if (_isInvoking)
            {
                _pendingChanges.Add((eventName, listener, true));
                return;
            }

            if (_activeListeners.TryGetValue(eventName, out var existing))
            {
                _activeListeners[eventName] = existing + listener;
            }
            else
            {
                _activeListeners[eventName] = listener;
            }
        }

        public void RemoveListener(HUJIEventName eventName, Action<object> listener)
        {
            if (_isInvoking)
            {
                _pendingChanges.Add((eventName, listener, false));
                return;
            }

            if (_activeListeners.TryGetValue(eventName, out var existing))
            {
                existing -= listener;
                if (existing == null)
                {
                    _activeListeners.Remove(eventName);
                }
                else
                {
                    _activeListeners[eventName] = existing;
                }
            }
        }

        public void InvokeEvent(HUJIEventName eventName, object obj)
        {
            if (!_activeListeners.TryGetValue(eventName, out var callback))
            {
                return;
            }

            _isInvoking = true;
            callback?.Invoke(obj);
            _isInvoking = false;

            ApplyPendingListeners();
        }

        private void ApplyPendingListeners()
        {
            if (_pendingChanges.Count == 0) return;

            for (int i = 0; i < _pendingChanges.Count; i++)
            {
                var (evt, listener, isAddition) = _pendingChanges[i];
                
                if (isAddition) AddListener(evt, listener);
                else RemoveListener(evt, listener);
            }

            _pendingChanges.Clear();
        }
    }

    public enum HUJIEventName
    {
        None,
        OnUpdatePlayerHealth,
        OnAbilityCd,
        OnOpenWindow,
        OnClickAlphaKey,
        OnUpdateActionSlot,
    }
}