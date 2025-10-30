using UnityEngine;
using UnityEngine.SceneManagement;
using Types;

namespace MainMenuLogic
{
    [ExecuteAlways]
    public class MenuManager : MonoBehaviour
    {
        private static MenuManager Instance;

        private GameObject mainMenu;
        private GameObject creditsMenu;
        private GameObject settingsMenu;
        private GameObject loadGameMenu;
        
        private void Awake()
        {
            Instance = this;
            if (!mainMenu) mainMenu = GameObject.Find(Tags.mainMenuTag);
            if (!creditsMenu) creditsMenu = GameObject.Find(Tags.creditsMenuTag);
            if (!settingsMenu) settingsMenu = GameObject.Find(Tags.settingsMenuTag);
            if (!loadGameMenu) loadGameMenu = GameObject.Find(Tags.loadGameMenuTag);
        }

        private void Update()
        {
            if (!Instance) Instance = this;
            if (!mainMenu) mainMenu = GameObject.Find(Tags.mainMenuTag);
            if (!creditsMenu) creditsMenu = GameObject.Find(Tags.creditsMenuTag);
            if (!settingsMenu) settingsMenu = GameObject.Find(Tags.settingsMenuTag);
            if (!loadGameMenu) loadGameMenu = GameObject.Find(Tags.loadGameMenuTag);
        }

        private void OnDestroy()
        {
            Instance = null;
        }
        
        public static void Play(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }

        public static void OpenSettings()
        {
            Instance.mainMenu?.SetActive(false);
            Instance.creditsMenu?.SetActive(false);
            Instance.settingsMenu?.SetActive(true);
            Instance.loadGameMenu?.SetActive(false);
        }

        public static void OpenCredits()
        {
            Instance.mainMenu?.SetActive(false);
            Instance.creditsMenu?.SetActive(true);
            Instance.settingsMenu?.SetActive(false);
            Instance.loadGameMenu?.SetActive(false);
        }

        public static void QuitGame()
        {
            Application.Quit();
        }

        public static void OpenLoadMenu()
        {
            Instance.mainMenu?.SetActive(false);
            Instance.creditsMenu?.SetActive(false);
            Instance.settingsMenu?.SetActive(false);
            Instance.loadGameMenu?.SetActive(true);
        }

        public static void BackButton()
        {
            Instance.mainMenu?.SetActive(true);
            Instance.creditsMenu?.SetActive(false);
            Instance.settingsMenu?.SetActive(false);
            Instance.loadGameMenu?.SetActive(false);
        }
    }
}