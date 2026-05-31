using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using Newtonsoft.Json;
using UnityEngine;

namespace HUJI
{
    public class HUJIConfigManager
    {
        private Action _onInitComplete;

        public HUJIConfigManager(Action onComplete)
        {
            _onInitComplete = onComplete;

            var defaults = new Dictionary<string, object>();
            FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults).ContinueWithOnMainThread(OnDefaultValuesSet);
        }

        private void OnDefaultValuesSet(Task task)
        {
            FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero).ContinueWithOnMainThread(OnFetchComplete);
        }

        private void OnFetchComplete(Task obj)
        {
            FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWithOnMainThread(task => _onInitComplete?.Invoke());
        }
        
        public void LoadConfigAsync<T>(string configId, Action<T> onComplete)
        {
            var rawJson = FirebaseRemoteConfig.DefaultInstance.GetValue(configId).StringValue;
            var deserializeObject = JsonConvert.DeserializeObject<T>(rawJson);

            onComplete.Invoke(deserializeObject);
        }

        #region Locally

        public void LoadLocalConfig<T>(HUJIConfigType configType, Action<T> onComplete)
        {
            TextAsset matchingConfig = Resources.Load<TextAsset>($"Configs/{configType.ToString()}");
            if (matchingConfig)
            {
                try
                {
                    var deserialized = JsonConvert.DeserializeObject<T>(matchingConfig.text);
                    onComplete?.Invoke(deserialized);
                }
                catch (Exception e)
                {
                    HUJIDebug.LogError($"Deserialization failed: {e.Message}\n{e.StackTrace}");
                    onComplete?.Invoke(default);
                }
            }
            else
            {
                HUJIDebug.LogError($"No file named '{configType}' found in any subfolder of Resources.");
                onComplete?.Invoke(default);
            }
        }

        #endregion

    }
    public enum HUJIConfigType
    {
        None,
        StatConfig,
    }
}