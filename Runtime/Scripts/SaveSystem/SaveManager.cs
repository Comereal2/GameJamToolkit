using System;
using System.Collections.Generic;
using System.Reflection;
using Attributes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaveSystem
{
    public class SaveManager : ScriptableObject
    {
        [Readonly] [SerializeField] private List<object> monitoredVariables = new();
        [Readonly] [SerializeField] private List<MonoScript> scannableScripts = new();
        
        #if UNITY_EDITOR
        public void ReValidateMonitoredScripts()
        {
            scannableScripts.Clear();
            
            string[] paths = AssetDatabase.GetAllAssetPaths();

            foreach (var path in paths)
            {
                if (!path.StartsWith("Packages") && path.EndsWith(".cs"))
                {
                    Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                    if (obj is MonoScript)
                    {
                        scannableScripts.Add((MonoScript)obj);
                    }
                }
            }
        }
        
        public void ReValidateMonitoredVariables()
        {
            monitoredVariables.Clear();
            
            Component[] allComponents = FindObjectsByType<Component>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (Component component in allComponents)
            {
                if (component is MonoBehaviour)
                {
                    if (scannableScripts.Contains(MonoScript.FromMonoBehaviour((MonoBehaviour)component)))
                    {
                        var fields = component.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                        foreach (var field in fields)
                        {
                            if (!Attribute.IsDefined(field, typeof(MonitorAttribute))) continue;
                            
                            string fieldName = field.Name;
                            object fieldValue = field.GetValue(component);
                            
                            monitoredVariables.Add(fieldValue);
                        }
                    }
                }
            }
        }
        #endif

        public void SaveVariablesState()
        {
            foreach (var variable in monitoredVariables)
            {
                //Save it to a file
            }
        }
    }
}