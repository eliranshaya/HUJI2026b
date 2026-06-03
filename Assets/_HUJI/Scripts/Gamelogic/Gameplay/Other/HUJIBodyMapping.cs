using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HUJI.Gamelogic
{
    [Serializable]
    public class HUJIBodyMapping
    {
        [SerializeField] private HUJIBodyPart[] _bodyParts;
        public HUJIBodyPart[] BodyParts => _bodyParts;

        [Serializable]
        public class HUJIBodyPart
        {
            public HUJIBodyType BodyType;
            public Transform Transform;
        }

#if UNITY_EDITOR
        [HUJIButton("SetBodyMapping")]
        public void SetBodyMapping(Transform t)
        {
            List<HUJIBodyPart> bodyParts = new();
            foreach (HUJIBodyType bodyType in Enum.GetValues(typeof(HUJIBodyType)))
            {
                if (bodyType == HUJIBodyType.None)
                {
                    continue;
                }

                var child = t.FindInChildrenByName(bodyType.ToString());
                if (child != null)
                {
                    bodyParts.Add(new HUJIBodyPart()
                    {
                        BodyType = bodyType,
                        Transform = child.transform
                    });
                }
            }

            _bodyParts = bodyParts.ToArray();
        }
#endif

        public Transform GetBodyTransform(HUJIBodyType bodyType)
        {
            var bodyPart = _bodyParts.FirstOrDefault(x => x.BodyType == bodyType);
            if (bodyPart != null)
            {
                return bodyPart.Transform;
            }

            HUJIDebug.Log($"{bodyType} not found");
            return null;
        }
    }
}