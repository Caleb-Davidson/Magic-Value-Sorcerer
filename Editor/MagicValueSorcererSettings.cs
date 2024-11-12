using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SettingsManagement;
using UnityEngine;

namespace Magic_Value_Sorcerer.Editor {
public static class MagicValueSorcererSettings {
    private const string ENABLED_KEY = "Enabled";
    
    public static event Action? OnSettingsChanged;
    
    private static readonly Settings instance = new("draco.magic-value-sorcerer");
    private static Dictionary<string, IUserSetting> settings = new();

    private static string GetFullKey(MagicValueSorcerer sorcerer, string key) =>
        $"{sorcerer.GetType().FullName}.{key}";
    
    public static T Get<T>(MagicValueSorcerer sorcerer, string key, T defaultValue) {
        var fullKey = GetFullKey(sorcerer, key);
        if (!settings.ContainsKey(fullKey)) {
            settings[fullKey] = new UserSetting<T>(instance, fullKey, defaultValue);
        }
        return ((UserSetting<T>)settings[fullKey]).value;
    }
    
    public static void Set<T>(MagicValueSorcerer sorcerer, string key, T value) {
        var fullKey = GetFullKey(sorcerer, key);
        if (!settings.ContainsKey(fullKey)) {
            settings[fullKey] = new UserSetting<T>(instance, fullKey, value);
        }
        ((UserSetting<T>)settings[fullKey]).value = value;
        OnSettingsChanged?.Invoke();
    }
    
    public static bool IsSorcererEnabled(MagicValueSorcerer sorcerer) => Get(sorcerer, ENABLED_KEY, true);
    public static void SetSorcererEnabled(MagicValueSorcerer sorcerer, bool enabled) => Set(sorcerer, ENABLED_KEY, enabled);
        
    [SettingsProvider]
    private static SettingsProvider CreateSettingsProvider() {
        return new SettingsProvider("Project/Magic Value Sorcerer", SettingsScope.Project) {
            label = "Magic Value Sorcerer",
            keywords = GetKeywords(),
            guiHandler = SettingsDrawer.Instance.Draw
        };
    }

    private static HashSet<string> GetKeywords() {
        var keywords = new HashSet<string>(new[] { "Magic", "Value", "Sorcerer" });
        foreach (var sorcerer in MagicValueSorcerer.All) {
            keywords.Add(sorcerer.DisplayName);
        }

        return keywords;
    }

    private class SettingsDrawer {
        public static SettingsDrawer Instance { get; } = new();

        private int selectedTab;
        private float tabButtonWidth;
        private float preferencesWindowWidth;
        private readonly MagicValueSorcerer[] sorcerers;
        private readonly string[] sorcererNames;

        private SettingsDrawer() {
            sorcerers = MagicValueSorcerer.All;
            sorcererNames = sorcerers.Select(sorcerer => ObjectNames.NicifyVariableName(sorcerer.DisplayName)).ToArray();
        }

        public void Draw(string searchContext) {
            EditorGUILayout.LabelField("Enable or Disable Sorcerer Generators", EditorStyles.boldLabel);
            var lineRect = EditorGUILayout.GetControlRect(false, 2);
            EditorGUI.DrawRect(lineRect, Color.grey);
            var guiColor = GUI.color;
            for (var i = 0; i < sorcerers.Length; i++) {
                var sorcerer = sorcerers[i];
                using (new EditorGUILayout.HorizontalScope()) {
                    var enabled = IsSorcererEnabled(sorcerer);
                    if (enabled && sorcerer.NeedsToGenerate()) {
                        GUI.color = new Color(0.9f, 0.2f, 0.2f);
                    }
                    var newEnabled = EditorGUILayout.Toggle(sorcererNames[i], enabled);
                    if (newEnabled != enabled) {
                        SetSorcererEnabled(sorcerer, newEnabled);
                    }
                    
                    if (GUILayout.Button("Force Generate")) {
                        sorcerer.Refresh(true);
                    }
                }
                GUI.color = guiColor;
            }
            
            EditorGUILayout.Space();

            using (new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Specific Sorcerer Generator Settings", EditorStyles.boldLabel);

                var currentTab = selectedTab;
                const int MAX_COLUMNS = 5;
                for (var i = 0; i < sorcererNames.Length; i += MAX_COLUMNS) {
                    var names = sorcererNames.Skip(i).Take(MAX_COLUMNS).ToArray();

                    var selectedColumn = -1;
                    if (selectedTab >= i && selectedTab < i + MAX_COLUMNS) {
                        selectedColumn = selectedTab % MAX_COLUMNS;
                    }
                    
                    selectedColumn = GUILayout.Toolbar(selectedColumn, names);
                    
                    if (selectedColumn != -1) {
                        selectedTab = i + selectedColumn;
                    }
                }
                if (currentTab != selectedTab) {
                    GUI.FocusControl(null);
                }
                
                lineRect = EditorGUILayout.GetControlRect(false, 2);
                EditorGUI.DrawRect(lineRect, Color.grey);
                EditorGUILayout.Space();
                
                var sorcerer = sorcerers[selectedTab];
                if (sorcerer.HasCustomSettings) {
                    sorcerer.DrawCustomSettings();
                } else {
                    EditorGUILayout.LabelField("No custom settings available for this sorcerer.");
                }
            }
            
            EditorGUILayout.Space();
            lineRect = EditorGUILayout.GetControlRect(false, 2);
            EditorGUI.DrawRect(lineRect, Color.grey);
            EditorGUILayout.Space();
            
            using (new EditorGUILayout.HorizontalScope()) {
                if (GUILayout.Button("Generate Needed")) {
                    Array.ForEach(MagicValueSorcerer.All, sorcerer => sorcerer.Refresh());
                }

                if (GUILayout.Button("Force Generate All")) {
                    Array.ForEach(MagicValueSorcerer.All, sorcerer => sorcerer.Refresh(true));
                }
            }
        }
    }
}
}