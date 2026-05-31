using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace HUJI.Gamelogic
{
    public class HUJILoaderComponent : HUJIMonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private TextMeshProUGUI _percentageText;

        private Coroutine _sliderCoroutine;

        private void Awake()
        {
            _slider.minValue = 0;
            _slider.maxValue = 1;

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            UpdateSlider(0);
            WaitForSeconds(0.05f, () =>
            {
                CoreSetup(GamelogicSetup);
            });
        }

        private void CoreSetup(Action onComplete) //25-35
        {
            StopAndStartCoroutine(ref _sliderCoroutine, HUJIExtensions.ChangeValueOverTimeEase(0, Random.Range(0.25f, 0.35f), Random.Range(0.25f, 0.75f), UpdateSlider));
            var manager = new HUJICoreManager();
            manager.LoadManager(() =>
            {
                onComplete?.Invoke();
            });
        }

        private void GamelogicSetup() //65 - 85
        {
            StopAndStartCoroutine(ref _sliderCoroutine, HUJIExtensions.ChangeValueOverTimeEase(0.5f, Random.Range(0.65f, 0.85f), Random.Range(0.25f, 0.75f), UpdateSlider));

            var gameLogic = new HUJIGamelogic();
            gameLogic.LoadManager(() =>
            {
                StopWithNullCheckCoroutine(ref _sliderCoroutine);
                UpdateSlider(1);

                SceneManager.LoadScene(sceneBuildIndex: 1);

                WaitForSeconds(2f, () =>
                {
                    Destroy(gameObject);
                });
            });
        }

        private void UpdateSlider(float value)
        {
            _slider.value = value;
            _percentageText.text = $"{(value * 100):N0}%";
        }
    }
}