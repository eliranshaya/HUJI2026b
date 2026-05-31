using System;
using Newtonsoft.Json;
using UnityEngine;

namespace HUJI
{
    public class HUJIConfigManager
    {
        public void LoadConfig<T>(HUJIConfigType configType, Action<T> onComplete)
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
    }
    public enum HUJIConfigType
    {
        None,
        StatConfig,
    }
}