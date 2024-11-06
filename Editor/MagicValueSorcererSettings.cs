using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SettingsManagement;
using UnityEngine;

namespace Magic_Value_Sorcerer.Editor {
public static class MagicValueSorcererSettings {
    private const string KEY_BASE = "MagicValueSorcerer";
    private const string ENABLED_KEY = "Enabled";
    
    private static readonly Settings instance = new("draco.magic-value-sorcerer");
    private static Dictionary<string, IUserSetting> settings = new();

    private static string GetFullKey(MagicValueSorcerer sorcerer, string key) =>
        $"{KEY_BASE}.{sorcerer.Assembly}.{sorcerer.Namespace}.{sorcerer.ClassName}.{key}";
    
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
    }
    
    public static bool IsSorcererEnabled(MagicValueSorcerer sorcerer) => Get(sorcerer, ENABLED_KEY, true);
    public static void SetSorcererEnabled(MagicValueSorcerer sorcerer, bool enabled) => Set(sorcerer, ENABLED_KEY, enabled);
        
    [SettingsProvider]
    private static SettingsProvider CreateSettingsProvider() {
        return new SettingsProvider("Preferences/Magic Value Sorcerer", SettingsScope.User) {
            label = "Magic Value Sorcerer",
            keywords = GetKeywords(),
            guiHandler = SettingsDrawer.Instance.Draw
        };
    }

    private static HashSet<string> GetKeywords() {
        var keywords = new HashSet<string>(new[] { "Magic", "Value", "Sorcerer" });
        foreach (var sorcerer in MagicValueSorcerer.All) {
            keywords.Add(sorcerer.ClassName);
        }

        return keywords;
    }

    private class SettingsDrawer {
        public static SettingsDrawer Instance { get; } = new();

        private int selectedTab;
        private readonly MagicValueSorcerer[] sorcerers;
        private readonly string[] sorcererNames;

        private SettingsDrawer() {
            sorcerers = MagicValueSorcerer.All;
            sorcererNames = sorcerers.Select(sorcerer => sorcerer.ClassName).ToArray();
        }

        public void Draw(string searchContext) {
            EditorGUILayout.LabelField("Enable or Disable Sorcerer Generators", EditorStyles.boldLabel);
            var lineRect = EditorGUILayout.GetControlRect(false, 2);
            EditorGUI.DrawRect(lineRect, Color.grey);
            var guiColor = GUI.color;
            foreach (var sorcerer in MagicValueSorcerer.All) {
                using (new EditorGUILayout.HorizontalScope()) {
                    var enabled = IsSorcererEnabled(sorcerer);
                    if (enabled && sorcerer.NeedsToGenerate()) {
                        GUI.color = new Color(0.9f, 0.2f, 0.2f);
                    }
                    var newEnabled = EditorGUILayout.Toggle(sorcerer.ClassName, enabled);
                    if (newEnabled != enabled) {
                        SetSorcererEnabled(sorcerer, newEnabled);
                    }
                    
                    if (GUILayout.Button("Force Generate")) {
                        MagicValueGenerator.Generate(sorcerer, true);
                    }
                }
                GUI.color = guiColor;
            }
            
            EditorGUILayout.Space();

            using (new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Specific Sorcerer Generator Settings", EditorStyles.boldLabel);
                
                selectedTab = GUILayout.Toolbar(selectedTab, sorcererNames);
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
                    MagicValueGenerator.GenerateAll();
                }

                if (GUILayout.Button("Force Generate All")) {
                    MagicValueGenerator.GenerateAll(true);
                }
            }
        }
    }
}
}