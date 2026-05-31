using System.IO;
using UnityEditor;
using UnityEngine;

namespace Core
{
    public class ChangeFileNames : EditorWindow
    {
        private string searchFor = "Core";
        private string replaceWith = "";

        [MenuItem("Tools/Core/Editor Tools/Change File Names", false, 2000)]
        public static void ShowWindow()
        {
            var window = GetWindow<ChangeFileNames>("Change File Names");
            if (window != null)
            {
                Vector2 windowSize = new Vector2(300, 100);
                window.minSize = windowSize;
                window.maxSize = windowSize;
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("Change File Names", EditorStyles.boldLabel);

            searchFor = EditorGUILayout.TextField("Search For", searchFor);
            replaceWith = EditorGUILayout.TextField("Replace With", replaceWith);

            if (GUILayout.Button("Change File Names"))
            {
                ChangeAllFileNames(searchFor, replaceWith);
            }
        }

        private static void ChangeAllFileNames(string searchFor, string replaceWith)
        {
            string[] allFiles = Directory.GetFiles(Application.dataPath, "*", SearchOption.AllDirectories);
            string[] allDirectories = Directory.GetDirectories(Application.dataPath, "*", SearchOption.AllDirectories);

            foreach (var filePath in allFiles)
            {
                string fileName = Path.GetFileName(filePath);
                if (fileName.Contains(searchFor))
                {
                    string newFileName = fileName.Replace(searchFor, replaceWith);
                    string newFilePath = Path.Combine(Path.GetDirectoryName(filePath), newFileName);
                    File.Move(filePath, newFilePath);
                }
            }

            foreach (var directoryPath in allDirectories)
            {
                string directoryName = Path.GetFileName(directoryPath);
                if (directoryName.Contains(searchFor))
                {
                    string newDirectoryName = directoryName.Replace(searchFor, replaceWith);
                    string newDirectoryPath = Path.Combine(Path.GetDirectoryName(directoryPath), newDirectoryName);
                    Directory.Move(directoryPath, newDirectoryPath);
                }
            }

            AssetDatabase.Refresh();
            Debug.Log("File names have been changed.");
        }
    }
}
