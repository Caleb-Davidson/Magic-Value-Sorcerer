using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Magic_Value_Sorcerer.Editor.Sorcerers {
[UsedImplicitly]
public class BuildScenesSorcerer : DefaultMagicValueSorcerer {
    public override string ClassName => "Scenes";
    private const string NAME_FIELD = "NAME";
    private const string PATH_FIELD = "PATH";
    private const string MODE_PARAM = "mode";

    protected override string Generate(ClassBuilder builder) {
        var sceneNameStrategy = Settings.GetSceneNameStrategy(this);
        var generateLoadMethods = Settings.GetGenerateLoadMethods(this);
        var loadMode = Settings.GetLoadMode(this);
        Settings.SetLastGenerationSettings(this, new LastGenerationSettings(sceneNameStrategy, generateLoadMethods, loadMode));
        
        builder.AddUsing("UnityEngine")
            .AddUsing("UnityEngine.SceneManagement");
        foreach (var (name, value) in GetScenes(sceneNameStrategy)) {
            var innerClassBuilder = builder.CreateInnerClassBuilder(name)
                .AddConstString(NAME_FIELD, name)
                .AddConstString(PATH_FIELD, value);

            if (generateLoadMethods) {
                innerClassBuilder
                    .AddMethod("Load", $"SceneManager.LoadScene({PATH_FIELD}, {MODE_PARAM});", $"LoadSceneMode {MODE_PARAM} = LoadSceneMode.{loadMode}")
                    .AddMethod("AsyncOperation", "LoadAsync", $"return SceneManager.LoadSceneAsync({PATH_FIELD}, {MODE_PARAM})!;", $"LoadSceneMode {MODE_PARAM} = LoadSceneMode.{loadMode}");
            }
        }
        return builder.Build();
    }

    public override bool NeedsToGenerate() {
        var existingPaths = SorcererUtils.GetInnerClasses(this)
            .Select(type => (string)type.GetField("PATH").GetValue(null))
            .ToList();
        var expectedPaths = EditorBuildSettings.scenes.Select(scene => scene.path).ToList();
        var lastGenerationSettings = Settings.GetLastGenerationSettings(this);
        return expectedPaths.Count != existingPaths.Count 
            || expectedPaths.Except(existingPaths).Any() 
            || lastGenerationSettings.SceneNameStrategy != Settings.GetSceneNameStrategy(this)
            || lastGenerationSettings.GenerateLoadMethods != Settings.GetGenerateLoadMethods(this)
            || lastGenerationSettings.LoadMode != Settings.GetLoadMode(this);
    }

    public override bool HasCustomSettings => true;
    public override void DrawCustomSettings() => Settings.Draw(this);
    
    private Dictionary<string, string> GetScenes(SceneNameStrategy sceneNameStrategy) {
        return sceneNameStrategy switch {
            SceneNameStrategy.ShortestUniqueSegment => GetShortestUniqueSegments(),
            SceneNameStrategy.FullPath => GetFullPath(),
            SceneNameStrategy.SceneName => GetSceneName(),
            _ => GetShortestUniqueSegments()
        };
        
        static Dictionary<string, string> GetShortestUniqueSegments() {
            var scenes = EditorBuildSettings.scenes;
            var nameToPathDictionary = new Dictionary<string, string>();
            var nameToScenePaths = new Dictionary<string, List<string>>();
        
            foreach (var scene in scenes) {
                var segments = Path.ChangeExtension(scene.path, null).Split('/').ToArray();
                // Add each of the possible paths to the dictionary so that we can count how many times each segment appears
                for (var i = 0; i < segments.Length; i++) {
                    var segment = string.Join("/", segments.Skip(i));
                    if (!nameToScenePaths.ContainsKey(segment)) {
                        nameToScenePaths[segment] = new List<string>();
                    }
                    nameToScenePaths[segment].Add(scene.path);
                }
            }

            foreach (var scene in scenes) {
                var segments = Path.ChangeExtension(scene.path, null).Split('/').ToArray();
                // Start from the end of the path and work backwards to find the shortest unique segment
                for (var i = segments.Length - 1; i >= 0; i++) {
                    var segment = string.Join("/", segments.Skip(i));
                    if (nameToScenePaths[segment].Count != 1) continue;
                    nameToPathDictionary[segment] = scene.path;
                    break;
                }
            }

            return nameToPathDictionary;
        }
    
        static Dictionary<string, string> GetFullPath() {
            return EditorBuildSettings.scenes.ToDictionary(scene => Path.ChangeExtension(scene.path, null), scene => scene.path);
        }
    
        static Dictionary<string, string> GetSceneName() {
            return EditorBuildSettings.scenes.ToDictionary(scene => Path.GetFileNameWithoutExtension(scene.path), scene => scene.path);
        }
    }
    
        private enum SceneNameStrategy {
        ShortestUniqueSegment,
        FullPath,
        SceneName
    }
    
    [Serializable]
    private struct LastGenerationSettings {
        public SceneNameStrategy SceneNameStrategy;
        public bool GenerateLoadMethods;
        public LoadSceneMode LoadMode;
        
        public LastGenerationSettings(SceneNameStrategy sceneNameStrategy, bool generateLoadMethods, LoadSceneMode loadMode) {
            SceneNameStrategy = sceneNameStrategy;
            GenerateLoadMethods = generateLoadMethods;
            LoadMode = loadMode;
        }
    }
    
    private static class Settings {
        private const string SCENE_NAME_STRATEGY_KEY = "SceneNameStrategy";
        private const string LOAD_MODE_KEY = "DefaultLoadMode";
        private const string GENERATE_LOAD_METHODS_KEY = "GenerateLoadMethods";
        private const string LAST_GENERATED_KEY = "LastGenerated";

        private const string SCENE_NAME_STRATEGY_TOOLTIP = @"What strategy to use when generating scene names?
Shortest Unique Segment: Find the shortest unique segment of the scene path that can be used as the scene name.
Full Path: Use the full path of the scene as the scene name.
Scene Name: Use the name of the scene as the scene name.";
        
        private const string GENERATE_LOAD_METHODS_TOOLTIP = "Whether or not to generate the Load and LoadAsync methods for each scene.";
        
        private const string LOAD_MODE_TOOLTIP = @"The default mode to load the scene in when not passed in.
Single: Closes the current scene and loads the new scene.
Additive: Load the scene additively.";
        
        public static void Draw(BuildScenesSorcerer self) {
            var sceneNameStrategy = GetSceneNameStrategy(self);
            EditorGUI.BeginChangeCheck();
            sceneNameStrategy = (SceneNameStrategy)EditorGUILayout.EnumPopup(new GUIContent("Scene Name Strategy", SCENE_NAME_STRATEGY_TOOLTIP), sceneNameStrategy);
            if (EditorGUI.EndChangeCheck()) {
                MagicValueSorcererSettings.Set(self, SCENE_NAME_STRATEGY_KEY, sceneNameStrategy);
            }
        
            var generateLoadMethods = GetGenerateLoadMethods(self);
            EditorGUI.BeginChangeCheck();
            generateLoadMethods = EditorGUILayout.Toggle(new GUIContent("Generate Load Methods", GENERATE_LOAD_METHODS_TOOLTIP), generateLoadMethods);
            if (EditorGUI.EndChangeCheck()) {
                MagicValueSorcererSettings.Set(self, GENERATE_LOAD_METHODS_KEY, generateLoadMethods);
            }

            if (!generateLoadMethods) return;
            
            var loadMode = GetLoadMode(self);
            EditorGUI.BeginChangeCheck();
            loadMode = (LoadSceneMode)EditorGUILayout.EnumPopup(new GUIContent("Default Load Mode", LOAD_MODE_TOOLTIP), loadMode);
            if (EditorGUI.EndChangeCheck()) {
                MagicValueSorcererSettings.Set(self, LOAD_MODE_KEY, loadMode);
            }
        }
        
        public static SceneNameStrategy GetSceneNameStrategy(BuildScenesSorcerer self) => MagicValueSorcererSettings.Get(self, SCENE_NAME_STRATEGY_KEY, SceneNameStrategy.ShortestUniqueSegment);
        public static bool GetGenerateLoadMethods(BuildScenesSorcerer self) => MagicValueSorcererSettings.Get(self, GENERATE_LOAD_METHODS_KEY, true);
        public static LoadSceneMode GetLoadMode(BuildScenesSorcerer self) => MagicValueSorcererSettings.Get(self, LOAD_MODE_KEY, LoadSceneMode.Single);
        public static LastGenerationSettings GetLastGenerationSettings(BuildScenesSorcerer self) => MagicValueSorcererSettings.Get(self, LAST_GENERATED_KEY, new LastGenerationSettings(SceneNameStrategy.ShortestUniqueSegment, true, LoadSceneMode.Single));
        public static void SetLastGenerationSettings(BuildScenesSorcerer self, LastGenerationSettings settings) => MagicValueSorcererSettings.Set(self, LAST_GENERATED_KEY, settings);
    }
}
}