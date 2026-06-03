using System;
using HUJI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUJIPlayerHealthUIComponent : HUJIMonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _text;

    private void Awake()
    {
        AddListener(HUJIEventName.OnUpdatePlayerHealth, OnUpdatePlayerHealth);
    }

    private void OnDestroy()
    {
        RemoveListener(HUJIEventName.OnUpdatePlayerHealth, OnUpdatePlayerHealth);
    }

    private void OnUpdatePlayerHealth(object obj)
    {
        var getType = obj.GetType();
        if (getType == typeof(ValueTuple<int, int>))
        {
            HUJIDebug.Log($"Tuple<int, int>");
        }

        HUJIDebug.Log($"{getType}");

        if (obj is (float currentHealth, float maxHealth))
        {
            HUJIDebug.Log($"float, float");
        }
        else if (obj is (float ch, int mh))
        {
            HUJIDebug.Log($"float, int");
        }
        else if (obj is (int c, int m))
        {
            HUJIDebug.Log($"int, int");
        }

        HUJIDebug.Log($"nothing");
    }
}