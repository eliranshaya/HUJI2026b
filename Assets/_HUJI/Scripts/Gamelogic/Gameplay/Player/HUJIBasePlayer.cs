using System;

namespace HUJI.Gamelogic
{
    public abstract class HUJIBasePlayer
    {
        protected HUJIPlayerPrefabComponent _player;
        protected HUJIGameManagerComponent _gameManager => _player.GameManager;
        protected HUJICoreManager CoreManager => HUJICoreManager.Instance;
        protected HUJIGamelogic Gamelogic => HUJIGamelogic.Instance;

        #region Events:

        protected void AddListener(HUJIEventName eventName, Action<object> eventCallback)
        {
            CoreManager?.EventManager.AddListener(eventName, eventCallback);
        }

        protected void RemoveListener(HUJIEventName eventName, Action<object> eventCallback)
        {
            CoreManager?.EventManager.RemoveListener(eventName, eventCallback);
        }

        protected void InvokeEvent(HUJIEventName eventName, object obj)
        {
            CoreManager?.EventManager.InvokeEvent(eventName, obj);
        }

        #endregion

        public virtual void OnInitialize(HUJIPlayerPrefabComponent player)
        {
            _player = player;
        }

        public virtual void OnDeath()
        {
        }

        public virtual void OnDestroy()
        {
        }
    }
}