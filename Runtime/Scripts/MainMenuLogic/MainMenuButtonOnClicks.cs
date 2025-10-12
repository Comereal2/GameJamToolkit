using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.Scripts.MainMenuLogic
{
    public class MainMenuButtonOnClicks
    {
        public static void Play(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }

        public static void OpenSettings()
        {
            
        }

        public static void OpenCredits()
        {
            
        }

        public static void QuitGame()
        {
            Application.Quit();
        }

        public static void OpenLoadMenu()
        {
            
        }
    }
}