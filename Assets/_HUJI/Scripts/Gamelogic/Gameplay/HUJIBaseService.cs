namespace HUJI.Gamelogic
{
    public abstract class HUJIBaseService
    {
        protected HUJIGameManagerComponent _gameManager;

        public virtual void OnAwake(HUJIGameManagerComponent gameManager)
        {
            _gameManager = gameManager;
        }

        public virtual void OnStart()
        {
        }

        public virtual void OnDestroy()
        {
        }
    }
}