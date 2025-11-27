using System;
using UnityEngine;

namespace Types
{
    public class Structs
    {
        [Serializable]
        public struct PlayerPrefData
        {
            public Enums.PlayerPrefsDataTypes DataType;
            public string Key;
            
            public object Value
            {
                get
                {
                    switch (DataType)
                    {
                        case Enums.PlayerPrefsDataTypes.Int:
                            return intValue;
                        case Enums.PlayerPrefsDataTypes.Float:
                            return floatValue;
                        case Enums.PlayerPrefsDataTypes.String:
                            return stringValue;
                        default:
                            return null;
                    }
                }
                set
                {
                    switch (DataType)
                    {
                        case Enums.PlayerPrefsDataTypes.Int:
                            intValue = (int)value;
                            break;
                        case Enums.PlayerPrefsDataTypes.Float:
                            floatValue = (float)value;
                            break;
                        case Enums.PlayerPrefsDataTypes.String:
                            stringValue = (string)value;
                            break;
                        default:
                            return;
                    }
                }
            }
            
            [SerializeField] private float floatValue;
            [SerializeField] private int intValue;
            [SerializeField] private string stringValue;

            //Used for nameof()
            public const string FloatValueSubfieldName = nameof(floatValue);
            public const string IntValueSubfieldName = nameof(intValue);
            public const string StringValueSubfieldName = nameof(stringValue);

            public PlayerPrefData(Enums.PlayerPrefsDataTypes dataType, string key, object value)
            {
                DataType = dataType;
                Key = key;
                floatValue = 0f;
                intValue = 0;
                stringValue = string.Empty;
                Value = value;
            }
        }
    }
}