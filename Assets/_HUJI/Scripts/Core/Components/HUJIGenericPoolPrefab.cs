using UnityEngine;

namespace HUJI
{
    public class HUJIGenericPoolPrefab : HUJIPoolPrefab
    {
        [HUJIBoxGroup("Settings")]
        [SerializeField] private float _delayToReturn = 2;

        protected Coroutine _returnCoroutine;

        public override void OnTakenFromPool()
        {
            base.OnTakenFromPool();

            StopAndStartWaitForSeconds(ref _returnCoroutine, _delayToReturn, () =>
            {
                HUJICoreManager.Instance.PoolManager.ReturnPoolPrefab(this);
            });
        }
    }
}