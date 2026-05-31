using UnityEngine;

namespace HUJI.Gamelogic
{
    [CreateAssetMenu(fileName = "StaticManager", menuName = "HUJI/StaticManager")]
    public class HUJIStaticManagerScriptableObject : ScriptableObject
    {
        public static readonly int ObstacleLayer = LayerMask.NameToLayer("Obstacle");
        public static readonly int PlayerMask = LayerMask.GetMask("Player");
    }
}