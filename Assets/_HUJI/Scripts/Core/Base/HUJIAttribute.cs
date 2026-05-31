using System;

namespace HUJI
{
    #region Base

    public abstract class HUJIAttribute : Attribute
    {
    }

    public abstract class HUJINamedAttribute : HUJIAttribute
    {
        public string Name { get; }

        protected HUJINamedAttribute(string name)
        {
            Name = name;
        }
    }

    #endregion

    #region Basic Display

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HUJIReadOnlyAttribute : HUJIAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HUJIHideLabelAttribute : HUJIAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HUJIHideInInspectorAttribute : HUJIAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HUJILabelTextAttribute : HUJINamedAttribute
    {
        public HUJILabelTextAttribute(string label) : base(label)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    public class HUJIHeaderAttribute : HUJINamedAttribute
    {
        public HUJIHeaderAttribute(string title) : base(title)
        {
        }
    }

    #endregion

    #region Visibility

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HUJIShowIfAttribute : HUJINamedAttribute
    {
        public HUJIShowIfAttribute(string conditionField) : base(conditionField)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HUJIHideIfAttribute : HUJINamedAttribute
    {
        public HUJIHideIfAttribute(string conditionField) : base(conditionField)
        {
        }
    }

    #endregion

    #region Layout / Grouping

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HUJIBoxGroupAttribute : HUJINamedAttribute
    {
        public HUJIBoxGroupAttribute(string groupName) : base(groupName)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HUJIFoldoutGroupAttribute : HUJINamedAttribute
    {
        public HUJIFoldoutGroupAttribute(string groupName) : base(groupName)
        {
        }
    }

    #endregion

    #region Validation

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HUJIRequiredAttribute : HUJIAttribute
    {
        public string Message { get; }

        public HUJIRequiredAttribute(string message = "This field is required.")
        {
            Message = message;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HUJIMinValueAttribute : HUJIAttribute
    {
        public float Min { get; }

        public HUJIMinValueAttribute(float min)
        {
            Min = min;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HUJIMaxValueAttribute : HUJIAttribute
    {
        public float Max { get; }

        public HUJIMaxValueAttribute(float max)
        {
            Max = max;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HUJIRangeAttribute : HUJIAttribute
    {
        public float Min { get; }
        public float Max { get; }

        public HUJIRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HUJIValidateInputAttribute : HUJIAttribute
    {
        public string MethodName { get; }
        public string Message { get; }

        public HUJIValidateInputAttribute(string methodName, string message = "Validation failed.")
        {
            MethodName = methodName;
            Message = message;
        }
    }

    #endregion

    #region Info / Feedback

    public enum HUJIInfoBoxType
    {
        Info,
        Warning,
        Error
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class HUJIInfoBoxAttribute : HUJIAttribute
    {
        public string Message { get; }
        public HUJIInfoBoxType Type { get; }

        public HUJIInfoBoxAttribute(string message, HUJIInfoBoxType type = HUJIInfoBoxType.Info)
        {
            Message = message;
            Type = type;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HUJIProgressBarAttribute : HUJIAttribute
    {
        public float Min { get; }
        public float Max { get; }
        public string ColorHex { get; }

        public HUJIProgressBarAttribute(float min, float max, string colorHex = "#00FF00")
        {
            Min = min;
            Max = max;
            ColorHex = colorHex;
        }
    }

    #endregion

    #region Interaction

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class HUJIButtonAttribute : HUJIAttribute
    {
        public string ButtonName { get; }
        public int Order { get; }

        public HUJIButtonAttribute(string buttonName, int order = 0)
        {
            ButtonName = buttonName;
            Order = order;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HUJIOnValueChangedAttribute : HUJINamedAttribute
    {
        public HUJIOnValueChangedAttribute(string methodName) : base(methodName)
        {
        }
    }

    #endregion
}