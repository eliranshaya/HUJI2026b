using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace HUJI
{
    public class HUJISaveManager
    {
        public void Save(IHUJISaveData saveData)
        {
            var saveId = saveData.GetType().FullName;
            var path = $"{Application.persistentDataPath}/{saveId}.HUJISave";
            var saveJson = JsonConvert.SerializeObject(saveData);
            File.WriteAllText(path, saveJson);
            
            HUJIDebug.Log($"Saved to: {path}");
        }

        public void Load<T>(Action<T> onComplete) where T : IHUJISaveData
        {
            string saveID = typeof(T).FullName;
            var path = $"{Application.persistentDataPath}/{saveID}.HUJISave";

            if (!File.Exists(path))
            {
                onComplete?.Invoke(default);
                return;
            }

            var saveJson = File.ReadAllText(path);
            var saveData = JsonConvert.DeserializeObject<T>(saveJson);
            
            onComplete.Invoke(saveData);
        }

        public void ClearAllData()
        {
            var path = Application.persistentDataPath;
            var files = Directory.GetFiles(path, "HUJISave");

            foreach (var file in files)
            {
                File.Delete(file);
            }

            PlayerPrefs.DeleteAll();
        }
    }
    public interface IHUJISaveData
    {
    }
}