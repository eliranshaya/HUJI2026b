using UnityEngine.SceneManagement;

namespace HUJI
{
    public class HUJICoreLoaderComponent : HUJIMonoBehaviour
    {
        private void Start()
        {
            var manager = new HUJICoreManager();
            manager.LoadCoreManagers(() =>
            {
                int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
                if (nextIndex < SceneManager.sceneCountInBuildSettings)
                {
                    SceneManager.LoadScene(nextIndex);
                }
                else
                {
                    HUJIDebug.LogWarning("No next scene in build settings.");
                }
            });
        }
    }
}