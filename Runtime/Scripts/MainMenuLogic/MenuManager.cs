using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Types;

namespace MainMenuLogic
{
    [ExecuteAlways]
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager Instance;

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
        
        public void Play(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }

        public void OpenSettings()
        {
            mainMenu.SetActive(false);
            creditsMenu.SetActive(false);
            settingsMenu.SetActive(true);
            loadGameMenu.SetActive(false);
        }

        public void OpenCredits()
        {
            mainMenu.SetActive(false);
            creditsMenu.SetActive(true);
            settingsMenu.SetActive(false);
            loadGameMenu.SetActive(false);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void OpenLoadMenu()
        {
            mainMenu.SetActive(false);
            creditsMenu.SetActive(false);
            settingsMenu.SetActive(false);
            loadGameMenu.SetActive(true);
        }

        public void BackButton()
        {
            mainMenu.SetActive(true);
            creditsMenu.SetActive(false);
            settingsMenu.SetActive(false);
            loadGameMenu.SetActive(false);
        }
    }
}