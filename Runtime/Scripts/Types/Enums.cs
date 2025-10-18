using System;
using System.Collections.Generic;
using PlasticGui;

namespace Types
{
    public class Enums
    {
        public enum MainMenuButtonTypes
        {
            None,
            Play,
            Settings,
            Credits,
            Quit,
            NewGame,
            LoadGame
        }
        
        public enum SettingsControlType
        {
            None,
            Button,
            Dropdown,
            InputField,
            Slider,
            Toggle
        }

        public enum PlayerPrefsDataTypes
        {
            None,
            Int,
            Float,
            String
        }

        public static Dictionary<PlayerPrefsDataTypes, Type> EnumDataToType { get; } = new()
        {
            { PlayerPrefsDataTypes.None, typeof(object) },
            { PlayerPrefsDataTypes.Int, typeof(int) },
            { PlayerPrefsDataTypes.Float, typeof(float) },
            { PlayerPrefsDataTypes.String, typeof(string) }
        };
    }
}