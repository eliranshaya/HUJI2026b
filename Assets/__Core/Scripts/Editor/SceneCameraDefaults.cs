using UnityEditor;

namespace Core
{
    [InitializeOnLoad]
    public static class SceneCameraDefaults
    {
        [MenuItem("Tools/Core/Editor Tools/SetCameraDefaults", priority = 100)]
        public static void SetCameraDefaults()
        {
            if (SceneView.lastActiveSceneView != null)
            {
                var sv = SceneView.lastActiveSceneView;

                sv.cameraSettings.fieldOfView = 60;
                sv.cameraSettings.nearClip = 0.1f;
                sv.cameraSettings.farClip = 1000f;

                sv.cameraSettings.speed = 1f;
                sv.cameraSettings.speedMin = 0.1f;
                sv.cameraSettings.speedMax = 10f;
            }
        }
    }
}