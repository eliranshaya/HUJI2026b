#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Core
{
    [InitializeOnLoad]
    public class ForceGameViewScale
    {
        static ForceGameViewScale()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                var gameView = EditorWindow.GetWindow(typeof(Editor).Assembly.GetType("UnityEditor.GameView"));
                var prop = gameView.GetType().GetField("m_ZoomArea",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (prop != null)
                {
                    var zoomArea = prop.GetValue(gameView);
                    var scaleProp = zoomArea.GetType().GetField("m_Scale",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (scaleProp != null)
                        scaleProp.SetValue(zoomArea, new Vector2(0.01f, 0.01f));
                }

                gameView.Repaint();
            }
        }
    }
}
#endif