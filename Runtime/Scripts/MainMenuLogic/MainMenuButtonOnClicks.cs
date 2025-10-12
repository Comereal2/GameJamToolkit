using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainMenuLogic
{
    public class MainMenuButtonOnClicks : MonoBehaviour
    {
        public void Play(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }

        public void OpenSettings()
        {
            
        }

        public void OpenCredits()
        {
            
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void OpenLoadMenu()
        {
            
        }
    }
}