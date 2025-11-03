using MainMenuLogic.MenuObjectDetectorScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using Types;

namespace MainMenuLogic
{
    [ExecuteAlways]
    public class MenuManager : MonoBehaviour
    {
        private static MenuManager Instance;

        private MainMenuScript mainMenu;
        private CreditsMenuScript creditsMenu;
        private SettingsMenuScript settingsMenu;
        private LoadGameMenuScript loadGameMenu;
        
        private void Awake()
        {
            Instance = this;
            if (!mainMenu) mainMenu = FindAnyObjectByType<MainMenuScript>();
            if (!creditsMenu) creditsMenu = FindAnyObjectByType<CreditsMenuScript>();
            if (!settingsMenu) settingsMenu = FindAnyObjectByType<SettingsMenuScript>();
            if (!loadGameMenu) loadGameMenu = FindAnyObjectByType<LoadGameMenuScript>();
        }

        private void Update()
        {
            if (!Instance) Instance = this;
            if (!mainMenu) mainMenu = FindAnyObjectByType<MainMenuScript>();
            if (!creditsMenu) creditsMenu = FindAnyObjectByType<CreditsMenuScript>();
            if (!settingsMenu) settingsMenu = FindAnyObjectByType<SettingsMenuScript>();
            if (!loadGameMenu) loadGameMenu = FindAnyObjectByType<LoadGameMenuScript>();
        }

        private void OnDestroy()
        {
            Instance = null;
        }
        
        public static void BackButton()
        {
            Instance.mainMenu?.gameObject.SetActive(true);
            Instance.creditsMenu?.gameObject.SetActive(false);
            Instance.settingsMenu?.gameObject.SetActive(false);
            Instance.loadGameMenu?.gameObject.SetActive(false);
        }
        
        #region MainMenuFunctions
        public static void Play(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }

        public static void OpenSettings()
        {
            Instance.mainMenu?.gameObject.SetActive(false);
            Instance.creditsMenu?.gameObject.SetActive(false);
            Instance.settingsMenu?.gameObject.SetActive(true);
            Instance.loadGameMenu?.gameObject.SetActive(false);
        }

        public static void OpenCredits()
        {
            Instance.mainMenu?.gameObject.SetActive(false);
            Instance.creditsMenu?.gameObject.SetActive(true);
            Instance.settingsMenu?.gameObject.SetActive(false);
            Instance.loadGameMenu?.gameObject.SetActive(false);
        }

        public static void QuitGame()
        {
            Application.Quit();
        }

        public static void OpenLoadMenu()
        {
            Instance.mainMenu?.gameObject.SetActive(false);
            Instance.creditsMenu?.gameObject.SetActive(false);
            Instance.settingsMenu?.gameObject.SetActive(false);
            Instance.loadGameMenu?.gameObject.SetActive(true);
        }
        #endregion
    }
}