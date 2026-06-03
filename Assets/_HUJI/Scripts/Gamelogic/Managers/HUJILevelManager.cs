using System;
using Newtonsoft.Json;
using UnityEngine;

namespace HUJI.Gamelogic
{
    public class HUJILevelManager
    {
        public const int WAVES_PER_LEVEL = 4;

        private HUJILevelData _data;
        public HUJILevelData Data => _data;

        public HUJILevelManager(Action onComplete)
        {
            HUJICoreManager.Instance.SaveManager.Load((HUJILevelData data) =>
            {
                _data = data ?? new();

                onComplete?.Invoke();
            });
        }

        public void OnClickBossFight()
        {
            if (_data.IsLoop)
            {
                _data.IsLoop = false;
                // HUJIManager.Instance.EventManager.InvokeEvent(HUJIEventName.OnChangeIsLoopState, null);
                _data.CurrentWave = Mathf.Clamp(_data.CurrentWave + 1, 1, WAVES_PER_LEVEL);
                _data.SaveData();

                // HUJICoreManager.Instance.EventManager.InvokeEvent(HUJIEventName.OnClickBossFight, null);
            }
        }

        public void OnPlayerDeath()
        {
            if (_data.IsLoop)
            {
                return;
            }

            if (_data.CurrentWave == WAVES_PER_LEVEL)
            {
                _data.CurrentWave--;
                _data.IsLoop = true;
                // HUJIManager.Instance.EventManager.InvokeEvent(HUJIEventName.OnChangeIsLoopState, null);
                _data.SaveData();
            }
        }

        public void AddLevelProgress()
        {
            if (_data.IsLoop)
            {
                return;
            }

            if (_data.CurrentWave < WAVES_PER_LEVEL)
            {
                _data.CurrentWave = Mathf.Clamp(_data.CurrentWave + 1, 1, WAVES_PER_LEVEL);
                _data.SaveData();
                return;
            }

            if (_data.MaxLevelNumber < _data.CurrentLevelNumber)
            {
                return;
            }

            _data.CurrentWave = 1;
            _data.CurrentLevelNumber++;
            _data.MaxLevelNumber = Math.Max(_data.MaxLevelNumber, _data.CurrentLevelNumber);

            _data.SaveData();
        }
    }
    public class HUJILevelData : IHUJISaveData
    {
        [JsonProperty("ml")] public int MaxLevelNumber = 1;
        [JsonProperty("cl")] public int CurrentLevelNumber = 1;
        [JsonProperty("cw")] public int CurrentWave = 1;
        [JsonProperty("il")] public bool IsLoop = false;
    }
}