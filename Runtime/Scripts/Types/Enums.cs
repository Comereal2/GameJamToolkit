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
    }
}