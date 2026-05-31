using System.Collections.Generic;
using Firebase.Analytics;
using UnityEngine;

namespace HUJI
{
    public class HUJIAnalyticsManager
    {
        public HUJIAnalyticsManager()
        {
            FirebaseAnalytics.SetUserId(SystemInfo.deviceUniqueIdentifier);
        }

        public void ReportEvent(HUJIEventType eventType)
        {
            ReportEvent(eventType, new Dictionary<HUJIDataKey, object>());
        }

        public void ReportEvent(HUJIEventType eventType, Dictionary<HUJIDataKey, object> data)
        {
            var paramsData = new List<Parameter>();
            foreach (var keyVal in data)
            {
                if (keyVal.Value == null)
                {
                    continue;
                }
                
                var objType = keyVal.Value.GetType();
                var keyName = keyVal.Key.ToString();
                if (objType == typeof(string))
                {
                    paramsData.Add(new Parameter(keyName, (string) keyVal.Value));
                }
                else if(objType == typeof(float))
                {   
                    paramsData.Add(new Parameter(keyName, (float) keyVal.Value));
                }
                else if(objType == typeof(int))
                {
                    paramsData.Add(new Parameter(keyName, (int) keyVal.Value));
                }
                else if(objType == typeof(bool))
                {
                    paramsData.Add(new Parameter(keyName, (bool) keyVal.Value ? 1 : 0));
                }
            }

            Parameter[] array = paramsData.ToArray();
            FirebaseAnalytics.LogEvent(eventType.ToString(), array);
        }
    }
    public enum HUJIEventType
    {
        None,
        huji_start_level,
        huji_complete_level,
    }
    public enum HUJIDataKey
    {
        None,
        huji_level,
    }
}