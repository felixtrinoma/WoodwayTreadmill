// ----------------------------------------------------------------------------
// Unite 2017 - Game Architecture with Scriptable Objects
// 
// Author: Ryan Hipple
// Date:   10/04/17
// ----------------------------------------------------------------------------

using UnityEngine;

namespace ScriptableObjectArchitecture.Variables
{
    [CreateAssetMenu]
    public class FloatVariable : ScriptableObject
    {

#if UNITY_EDITOR
        [Multiline]
        public string DeveloperDescription = "";
#endif
        public float Value;

        public bool resetOnAwake=true;
        public float maxVal;
        public float minVal;

        public void SetValue(float value)
        {
            if (value > maxVal)
                maxVal = value;
            if (value < minVal)
                minVal = value;

            Value = value;
        }

        public void SetValue(FloatVariable value)
        {
            if (value.Value > maxVal)
                maxVal = value.Value;
            if (value.Value < minVal)
                minVal = value.Value;

            Value = value.Value;
        }

        public void ApplyChange(float amount)
        {
            Value += amount;
        }

        public void ApplyChange(FloatVariable amount)
        {
            Value += amount.Value;
        }


        public void Awake()
        {
            if (resetOnAwake)
            {
                minVal = 0;
                maxVal = 0;
            }
        }

    }
}