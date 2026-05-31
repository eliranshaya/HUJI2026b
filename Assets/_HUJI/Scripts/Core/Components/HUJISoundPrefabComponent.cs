using UnityEngine;

namespace HUJI
{
    public class HUJISoundPrefabComponent : HUJIPoolPrefab
    {
        [SerializeField] private AudioSource _audioSource;

        protected Coroutine _returnCoroutine;

        private void Reset()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void SetSound(AudioClip clip, float volume, float pitch)
        {
            _audioSource.clip = clip;
            _audioSource.volume = volume;
            _audioSource.pitch = pitch;
            _audioSource.Play();

            float clipLength = _audioSource.clip.length;
            StopAndStartWaitForSeconds(ref _returnCoroutine, clipLength + 0.05f, () =>
            {
                HUJICoreManager.Instance.PoolManager.ReturnPoolPrefab(this);
            });
        }

        public void StopSound()
        {
            _audioSource.Stop();
        }
    }
}