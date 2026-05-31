using System;

namespace HUJI
{
    public class HUJIMonoManagerObject : HUJIMonoBehaviour
    {
        public static HUJIMonoManagerObject Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}