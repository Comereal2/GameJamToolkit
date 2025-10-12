using UnityEditor;
using Editor.MainMenuCreator;

namespace Editor.TopBarConnector
{
    public class TopBarConnector
    {
        [MenuItem("Tools/Custom Main Menu/Main Menu", priority = 1)]
        private static void DisplayCreateMainMenu()
        {
            EditorWindow.GetWindow<MainMenuCreatorEditorWindow>("Main Menu Creator").Show();
        }
        
        [MenuItem("Tools/Custom Main Menu/Settings Menu", priority = 2)] 
        
        private static void DisplayCreateSettingsMenu()
        {
            EditorWindow.GetWindow<SettingsMenuCreatorEditorWindow>("Settings Menu Creator").Show();
        }

        [MenuItem("Tools/Custom Main Menu/Credits Menu", priority = 3)]
        private static void DisplayCreateCreditsMenu()
        {
            EditorWindow.GetWindow<CreditsMenuCreatorEditorWindow>("Credits Menu Creator").Show();
        }
    }
}