using UnityEngine;

namespace HUJI
{
    public class HUJIPoolPrefab : HUJIMonoBehaviour
    {
        [field: SerializeField] public HUJIPoolName PoolName { get; private set; }

#if UNITY_EDITOR
        public void SetPoolName(HUJIPoolName poolName)
        {
            PoolName = poolName;
        }
#endif

        public virtual void OnReturnedToPool()
        {
            gameObject.SetActive(false);
        }

        public virtual void OnTakenFromPool()
        {
            gameObject.SetActive(true);
        }

        public virtual void PreDestroy()
        {
        }
    }
}