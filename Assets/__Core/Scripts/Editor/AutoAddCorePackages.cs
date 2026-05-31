using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace Core
{
    public static class AutoAddCorePackages
    {
        private static readonly Queue<string> _packagesToAdd = new();
        private static AddRequest _addRequest;

        [MenuItem("Tools/Core/Editor Tools/Add Core Packages", priority = 100)]
        public static void AddCorePackages()
        {
            var packages = new List<string>
            {
                "com.unity.nuget.newtonsoft-json",
                //    "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask", //By URL
                "https://github.com/mob-sakai/UIEffect.git?path=Packages/src#5.7.0",
                "https://github.com/mob-sakai/ParticleEffectForUGUI.git#4.9.0",
                "https://github.com/mob-sakai/SoftMaskForUGUI.git?path=Packages/src#3.2.0",
                "https://github.com/CandyCoded/HapticFeedback.git#v1.0.3", // for haptic feedback
            };

            foreach (var package in packages)
            {
                if (!IsAlreadyInstalled(package))
                {
                    _packagesToAdd.Enqueue(package);
                }
                else
                {
                    Debug.Log($"Package already installed: {package}");
                }
            }

            TryAddNextPackage();
        }

        private static void TryAddNextPackage()
        {
            if (_packagesToAdd.Count == 0)
                return;

            string nextPackage = _packagesToAdd.Dequeue();
            Debug.Log($"Adding package: {nextPackage}");
            _addRequest = Client.Add(nextPackage);
            EditorApplication.update += PackageAddProgress;
        }

        private static void PackageAddProgress()
        {
            if (!_addRequest.IsCompleted) return;

            if (_addRequest.Status == StatusCode.Success)
            {
                Debug.Log($"✅ Successfully added: {_addRequest.Result.packageId}");
            }
            else
            {
                Debug.LogError($"❌ Failed to add package: {_addRequest.Error.message}");
            }

            EditorApplication.update -= PackageAddProgress;
            TryAddNextPackage();
        }

        private static bool IsAlreadyInstalled(string identifier)
        {
            string manifestPath = Path.Combine(Application.dataPath, "../Packages/manifest.json");
            if (!File.Exists(manifestPath))
            {
                return false;
            }

            string manifestText = File.ReadAllText(manifestPath);
            return manifestText.Contains("com.unity.ide.cursor") || manifestText.Contains("com.unity.nuget.newtonsoft-json");
        }
    }
}