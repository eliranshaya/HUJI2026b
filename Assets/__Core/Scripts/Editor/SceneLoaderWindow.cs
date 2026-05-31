using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Scene loader window — dockable replacement for the old toolbar injector.
    /// Open via: Window > Scene Loader (or Ctrl+Shift+L).
    /// </summary>
    public class SceneLoaderWindow : EditorWindow
    {
        // EditorPrefs keys (kept compatible with the old injector)
        const string k_OnlyBuildScenesPref = "SceneLoaderToolbar_IsAllScenes";
        const string k_PlayFromZeroPref = "SceneLoaderToolbar_PlayFromZero";
        const string k_SelectedScenePref = "SceneLoaderToolbar_SelectedScene";
        const string k_LastScenePref = "QG_LastScenePath";

        string[] _scenePaths = new string[0];
        string[] _sceneNames = new string[0];
        Vector2 _scroll;

        [MenuItem("Tools/Core/Editor Tools/Scene Loader %#l")] // Ctrl/Cmd + Shift + L
        public static void Open()
        {
            var window = GetWindow<SceneLoaderWindow>("Scene Loader");
            window.minSize = new Vector2(280, 120);
            window.Show();
        }

        // ---------- prefs ----------
        static bool OnlyBuildScenes
        {
            get => EditorPrefs.GetBool(k_OnlyBuildScenesPref, false);
            set => EditorPrefs.SetBool(k_OnlyBuildScenesPref, value);
        }
        static bool PlayFromZero
        {
            get => EditorPrefs.GetBool(k_PlayFromZeroPref, false);
            set => EditorPrefs.SetBool(k_PlayFromZeroPref, value);
        }
        static int SelectedIndex
        {
            get => EditorPrefs.GetInt(k_SelectedScenePref, 0);
            set => EditorPrefs.SetInt(k_SelectedScenePref, value);
        }

        // ---------- lifecycle ----------
        void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorBuildSettings.sceneListChanged += RefreshScenes;
            RefreshScenes();
            ApplyPlayFromZero();
        }

        void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorBuildSettings.sceneListChanged -= RefreshScenes;
        }

        void OnProjectChange() => RefreshScenes();

        // ---------- scene cache ----------
        void RefreshScenes()
        {
            if (OnlyBuildScenes)
            {
                _scenePaths = EditorBuildSettings.scenes
                    .Where(x => System.IO.File.Exists(x.path))
                    .Select(x => x.path)
                    .ToArray();
            }
            else
            {
                _scenePaths = AssetDatabase.FindAssets("t:Scene")
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Where(p => p.StartsWith("Assets") && System.IO.File.Exists(p))
                    .ToArray();
            }

            _sceneNames = _scenePaths
                .Select(p => AssetDatabase.LoadAssetAtPath<SceneAsset>(p)?.name)
                .Select(n => string.IsNullOrEmpty(n) ? "<missing>" : n)
                .ToArray();

            Repaint();
        }

        // ---------- play-from-zero ----------
        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode && PlayFromZero)
            {
                var activeScene = EditorSceneManager.GetActiveScene();
                var current = activeScene.path;
                if (!string.IsNullOrEmpty(current))
                    EditorPrefs.SetString(k_LastScenePref, current);

                // If the current scene has unsaved changes, prompt to save BEFORE
                // Unity swaps to playModeStartScene (which would otherwise discard them).
                if (activeScene.isDirty)
                {
                    bool saved = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    if (!saved)
                    {
                        // User picked Cancel — abort entering play mode so their work isn't lost.
                        EditorApplication.isPlaying = false;
                    }
                }
            }

            if (state == PlayModeStateChange.EnteredEditMode)
            {
                var last = EditorPrefs.GetString(k_LastScenePref, "");
                if (!string.IsNullOrEmpty(last) && System.IO.File.Exists(last))
                {
                    EditorSceneManager.OpenScene(last, OpenSceneMode.Single);
                    EditorPrefs.DeleteKey(k_LastScenePref);
                }
            }
        }

        static void ApplyPlayFromZero()
        {
            if (PlayFromZero)
            {
                var scenes = EditorBuildSettings.scenes;
                if (scenes.Length > 0 && !string.IsNullOrEmpty(scenes[0].path))
                {
                    var asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenes[0].path);
                    EditorSceneManager.playModeStartScene = asset;
                    if (asset == null)
                        Debug.LogWarning("[SceneLoader] Could not load scene at build index 0.");
                }
                else
                {
                    EditorSceneManager.playModeStartScene = null;
                    Debug.LogWarning("[SceneLoader] No scenes in Build Settings. Add one to use 'Play From 0'.");
                }
            }
            else
            {
                EditorSceneManager.playModeStartScene = null;
            }
        }

        // Re-apply on every script reload so the setting survives domain reloads.
        [InitializeOnLoadMethod]
        static void OnLoad()
        {
            EditorApplication.delayCall += ApplyPlayFromZero;
        }

        // ---------- scene ops ----------
        void OpenSceneAt(int index)
        {
            if (index < 0 || index >= _scenePaths.Length) return;
            var path = _scenePaths[index];
            if (!System.IO.File.Exists(path))
            {
                Debug.LogError("[SceneLoader] Scene path is invalid: " + path);
                return;
            }

            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(path);
        }

        void PlayFromSceneAt(int index)
        {
            if (index < 0 || index >= _scenePaths.Length) return;
            var current = EditorSceneManager.GetActiveScene().path;
            EditorPrefs.SetString(k_LastScenePref, current);
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(_scenePaths[index]);
            EditorApplication.isPlaying = true;
        }

        // ---------- GUI ----------
        void OnGUI()
        {
            EditorGUILayout.Space(4);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();
                bool onlyBuild = EditorGUILayout.ToggleLeft(new GUIContent("Only Build Scenes", "Limit the dropdown to scenes listed in Build Settings."),
                    OnlyBuildScenes);
                if (EditorGUI.EndChangeCheck())
                {
                    OnlyBuildScenes = onlyBuild;
                    RefreshScenes();
                }

                EditorGUI.BeginChangeCheck();
                bool playFromZero = EditorGUILayout.ToggleLeft(new GUIContent("Play From 0", "When ON, Unity's Play button always starts from the scene at build index 0."),
                    PlayFromZero);
                if (EditorGUI.EndChangeCheck())
                {
                    PlayFromZero = playFromZero;
                    ApplyPlayFromZero();
                }
            }

            EditorGUILayout.Space(4);

            if (_sceneNames.Length == 0)
            {
                EditorGUILayout.HelpBox(OnlyBuildScenes
                        ? "No scenes in Build Settings. Add some via File > Build Settings."
                        : "No scenes found in the project.",
                    MessageType.Info);
                if (GUILayout.Button("Refresh")) RefreshScenes();
                return;
            }

            int selected = Mathf.Clamp(SelectedIndex, 0, _sceneNames.Length - 1);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Scene", GUILayout.Width(50));
                int newSelected = EditorGUILayout.Popup(selected, _sceneNames);
                if (newSelected != selected)
                {
                    SelectedIndex = newSelected;
                    selected = newSelected;
                }

                if (GUILayout.Button("◀", GUILayout.Width(28)))
                {
                    SelectedIndex = Mathf.Max(0, selected - 1);
                    OpenSceneAt(SelectedIndex);
                }

                if (GUILayout.Button("▶", GUILayout.Width(28)))
                {
                    SelectedIndex = Mathf.Min(_scenePaths.Length - 1, selected + 1);
                    OpenSceneAt(SelectedIndex);
                }
            }

            EditorGUILayout.Space(4);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Open", GUILayout.Height(24)))
                    OpenSceneAt(SelectedIndex);

                if (GUILayout.Button("Play From", GUILayout.Height(24)))
                    PlayFromSceneAt(SelectedIndex);
            }

            EditorGUILayout.Space(8);

            // Quick-access list
            EditorGUILayout.LabelField("All Scenes", EditorStyles.boldLabel);
            using (var sv = new EditorGUILayout.ScrollViewScope(_scroll))
            {
                _scroll = sv.scrollPosition;
                for (int i = 0; i < _sceneNames.Length; i++)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        var style = (i == selected) ? EditorStyles.boldLabel : EditorStyles.label;
                        EditorGUILayout.LabelField(_sceneNames[i], style);
                        if (GUILayout.Button("Open", GUILayout.Width(50)))
                        {
                            SelectedIndex = i;
                            OpenSceneAt(i);
                        }

                        if (GUILayout.Button("Play", GUILayout.Width(50)))
                        {
                            SelectedIndex = i;
                            PlayFromSceneAt(i);
                        }
                    }
                }
            }
        }
    }
}