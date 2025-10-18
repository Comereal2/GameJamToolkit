using UnityEditor;
using Editor.MainMenuCreator;

namespace Editor.TopBarConnector
{
    public class TopBarConnector
    {
        private const string menuName = "Toolkit";
        private const string uiEditorName = "/Custom Menu";
        
        [MenuItem(menuName + uiEditorName + "/Main Menu", priority = 1)]
        private static void DisplayCreateMainMenu()
        {
            EditorWindow.GetWindow<MainMenuCreatorEditorWindow>("Main Menu Creator").Show();
        }
        
        [MenuItem(menuName + uiEditorName + "/Settings Menu", priority = 2)] 
        
        private static void DisplayCreateSettingsMenu()
        {
            EditorWindow.GetWindow<SettingsMenuCreatorEditorWindow>("Settings Menu Creator").Show();
        }

        [MenuItem(menuName + uiEditorName + "/Credits Menu", priority = 3)]
        private static void DisplayCreateCreditsMenu()
        {
            EditorWindow.GetWindow<CreditsMenuCreatorEditorWindow>("Credits Menu Creator").Show();
        }

        [MenuItem(menuName + uiEditorName + "/Load Game Menu", priority = 4)]
        private static void DisplayCreateLoadGameMenu()
        {
            EditorWindow.GetWindow<LoadGameWindowEditorWindow>("Load Game Window Creator").Show();
        }
    }
}