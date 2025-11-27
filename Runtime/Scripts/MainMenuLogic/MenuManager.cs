using System;
using System.Collections.Generic;
using System.Linq;
using MainMenuLogic.MenuObjectDetectorScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using Types;
using UnityEngine.UI;

namespace MainMenuLogic
{
    [ExecuteAlways]
    public class MenuManager : MonoBehaviour
    {
        [Serializable]
        private struct SettingsOption
        {
            public GameObject Obj;
            public Enums.SettingsControlType ControlType;
            public Structs.PlayerPrefData PlayerPref;

            public SettingsOption(GameObject obj, Enums.SettingsControlType controlType, Structs.PlayerPrefData playerPref)
            {
                Obj = obj;
                ControlType = controlType;
                PlayerPref = playerPref;
            }

            public SettingsOption(GameObject obj, Enums.SettingsControlType controlType, Enums.PlayerPrefsDataTypes dataType, string key, object value)
            {
                Obj = obj;
                ControlType = controlType;
                PlayerPref = new Structs.PlayerPrefData(dataType, key, value);
            }
        }

        private static MenuManager Instance;

        public MainMenuScript mainMenu { get; private set; }
        public CreditsMenuScript creditsMenu { get; private set; }
        public SettingsMenuScript settingsMenu { get; private set; }
        public LoadGameMenuScript loadGameMenu { get; private set; }
        
        [SerializeField] private List<SettingsOption> floatPlayerPrefs;
        [SerializeField] private List<SettingsOption> intPlayerPrefs;
        [SerializeField] private List<SettingsOption> stringPlayerPrefs;
        
        private void Awake()
        {
            RevalidateInstances();
        }

        private void Update()
        {
            RevalidateInstances();
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public void RevalidateInstances()
        {
            if (!Instance) Instance = this;
            if (!mainMenu) mainMenu = FindAnyObjectByType<MainMenuScript>(FindObjectsInactive.Include);
            if (!creditsMenu) creditsMenu = FindAnyObjectByType<CreditsMenuScript>(FindObjectsInactive.Include);
            if (!settingsMenu) settingsMenu = FindAnyObjectByType<SettingsMenuScript>(FindObjectsInactive.Include);
            if (!loadGameMenu) loadGameMenu = FindAnyObjectByType<LoadGameMenuScript>(FindObjectsInactive.Include);
        }
        
        /// <summary>
        /// Ordered by float -> int -> string
        /// </summary>
        public List<(string, Type)> GetAllKeys()
        {
            var list = floatPlayerPrefs.Select(pref => new ValueTuple<string, Type>(pref.PlayerPref.Key, typeof(float))).ToList();
            list.AddRange(intPlayerPrefs.Select(pref => new ValueTuple<string, Type>(pref.PlayerPref.Key, typeof(int))));
            list.AddRange(stringPlayerPrefs.Select(pref => new ValueTuple<string, Type>(pref.PlayerPref.Key, typeof(string))));

            return list;
        }
        
        /// <param name="index">Goes by the order from <see cref="GetAllKeys"/></param>
        public void RemovePlayerPref(int index)
        {
            if (index >= floatPlayerPrefs.Count) index -= floatPlayerPrefs.Count;
            else
            {
                floatPlayerPrefs.RemoveAt(index);
                return;
            }

            if (index >= intPlayerPrefs.Count) index -= intPlayerPrefs.Count;
            else
            {
                intPlayerPrefs.RemoveAt(index);
                return;
            }
            
            stringPlayerPrefs.RemoveAt(index);
        }
        
        public static void BackButton()
        {
            Instance.mainMenu?.gameObject.SetActive(true);
            Instance.creditsMenu?.gameObject.SetActive(false);
            Instance.settingsMenu?.gameObject.SetActive(false);
            if(Instance.settingsMenu && Instance.settingsMenu.isActiveAndEnabled) SaveSettings();
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

            foreach (var pref in Instance.floatPlayerPrefs)
            {
                SetFloatOptionValue(pref);
            }

            foreach (var pref in Instance.intPlayerPrefs)
            {
                SetIntOptionValue(pref);
            }

            foreach (var pref in Instance.stringPlayerPrefs)
            {
                SetStringOptionValue(pref);
            }
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

        #region SettingsMenuFunctions

        public static void AddSettingsOption(GameObject optionObject, Enums.SettingsControlType controlType, Structs.PlayerPrefData playerPref)
        {
            switch (playerPref.DataType)
            {
                case Enums.PlayerPrefsDataTypes.Int:
                    Instance.intPlayerPrefs.Add(new(optionObject, controlType, playerPref));
                    PlayerPrefs.SetInt(playerPref.Key, (int)playerPref.Value);
                    break;
                case Enums.PlayerPrefsDataTypes.Float:
                    Instance.floatPlayerPrefs.Add(new (optionObject, controlType, playerPref));
                    PlayerPrefs.SetFloat(playerPref.Key, (float)playerPref.Value);
                    break;
                case Enums.PlayerPrefsDataTypes.String:
                    Instance.stringPlayerPrefs.Add(new(optionObject, controlType, playerPref));
                    PlayerPrefs.SetString(playerPref.Key, (string)playerPref.Value);
                    break;
            }
        }

        public static void SaveSettings()
        {
            ClearPlayerPrefs();
            foreach (var playerPref in Instance.floatPlayerPrefs)
            {
                PlayerPrefs.SetFloat(playerPref.PlayerPref.Key, (float)GetValueFromObject(playerPref.Obj, playerPref.ControlType));
            }
            
            foreach (var playerPref in Instance.intPlayerPrefs)
            {
                PlayerPrefs.SetInt(playerPref.PlayerPref.Key, (int)GetValueFromObject(playerPref.Obj, playerPref.ControlType));
            }
            
            foreach (var playerPref in Instance.stringPlayerPrefs)
            {
                PlayerPrefs.SetString(playerPref.PlayerPref.Key, (string)GetValueFromObject(playerPref.Obj, playerPref.ControlType));
            }
            PlayerPrefs.Save();
        }

        private static void SetFloatOptionValue(SettingsOption option)
        {
            switch (option.ControlType)
            {
                case Enums.SettingsControlType.InputField:
                    option.Obj.GetComponent<InputField>().text = PlayerPrefs.GetFloat(option.PlayerPref.Key, (float)option.PlayerPref.Value).ToString();
                    break;
                case Enums.SettingsControlType.Slider:
                    option.Obj.GetComponent<Slider>().value = PlayerPrefs.GetFloat(option.PlayerPref.Key, (float)option.PlayerPref.Value);
                    break;
            }
        }
        
        private static void SetIntOptionValue(SettingsOption option)
        {
            switch (option.ControlType)
            {
                case Enums.SettingsControlType.Dropdown:
                    option.Obj.GetComponent<Dropdown>().value = PlayerPrefs.GetInt(option.PlayerPref.Key, (int)option.PlayerPref.Value);
                    break;
                case Enums.SettingsControlType.InputField:
                    option.Obj.GetComponent<InputField>().text = PlayerPrefs.GetInt(option.PlayerPref.Key, (int)option.PlayerPref.Value).ToString();
                    break;
                case Enums.SettingsControlType.Slider:
                    option.Obj.GetComponent<Slider>().value = PlayerPrefs.GetInt(option.PlayerPref.Key, (int)option.PlayerPref.Value);
                    break;
                case Enums.SettingsControlType.Toggle:
                    option.Obj.GetComponent<Toggle>().isOn = PlayerPrefs.GetInt(option.PlayerPref.Key, (int)option.PlayerPref.Value) == 1;
                    break;
            }
        }

        private static void SetStringOptionValue(SettingsOption option)
        {
            if (option.ControlType == Enums.SettingsControlType.InputField) option.Obj.GetComponent<InputField>().text = PlayerPrefs.GetString(option.PlayerPref.Key, (string)option.PlayerPref.Value);
        }
        
        private static object GetValueFromObject(GameObject obj, Enums.SettingsControlType type)
        {
            return type switch
            {
                Enums.SettingsControlType.Dropdown => obj.GetComponent<Dropdown>().value,
                Enums.SettingsControlType.InputField => obj.GetComponent<InputField>().text,
                Enums.SettingsControlType.Slider => obj.GetComponent<Slider>().value,
                Enums.SettingsControlType.Toggle => obj.GetComponent<Toggle>().isOn ? 1 : 0,
                _ => null
            };
        }

        public static List<object> LoadSettings(List<string> keys)
        {
            List<object> values = new();
            foreach (var key in keys)
            {
                if (!PlayerPrefs.HasKey(key))
                {
                    Debug.LogWarning($"Tried to access player prefs value with incorrect key - {key}");
                    values.Add(-1);
                    continue;
                }

                var val = PlayerPrefs.GetFloat(key, float.NaN);
                if (float.IsNaN(val)) val = PlayerPrefs.GetInt(key, int.MinValue + 2);
                if ((int)val == int.MinValue + 2) values.Add(PlayerPrefs.GetString(key));
                else values.Add(val);
            }
            return values;
        }

        public static List<float> LoadFloatSettings(List<string> keys) => keys.Select(key => PlayerPrefs.GetFloat(key, -1)).ToList();
        
        public static List<int> LoadIntSettings(List<string> keys) => keys.Select(key => PlayerPrefs.GetInt(key, -1)).ToList();

        public static List<string> LoadStringSettings(List<string> keys) => keys.Select(key => PlayerPrefs.GetString(key)).ToList();
        
        public static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            Instance.floatPlayerPrefs.Clear();
            Instance.intPlayerPrefs.Clear();
            Instance.stringPlayerPrefs.Clear();
        }

        #endregion
    }
}