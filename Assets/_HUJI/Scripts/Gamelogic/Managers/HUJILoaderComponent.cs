using System;
using UnityEngine.SceneManagement;

namespace HUJI.Gamelogic
{
    public class HUJILoaderComponent : HUJIMonoBehaviour
    {
        
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            WaitForSeconds(0.05f, () =>
            {
                CoreSetup(GamelogicSetup);
            });
        }

        private void CoreSetup(Action onComplete)
        {
            var manager = new HUJICoreManager();
            manager.LoadCoreManagers(() =>
            {
                onComplete?.Invoke();
            });
        }

        private void GamelogicSetup()
        {
            var gameLogic = new HUJIGamelogic();
            gameLogic.LoadManager(() =>
            {
                SceneManager.LoadScene(sceneBuildIndex: 1);
                WaitForSeconds(2f, () =>
                {
                    Destroy(gameObject);
                });
            });
        }
    }
}