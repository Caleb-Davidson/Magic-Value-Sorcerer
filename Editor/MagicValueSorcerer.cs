using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Magic_Value_Sorcerer.Editor {
public abstract class MagicValueSorcerer {
    public abstract string DisplayName { get; }
    public virtual bool HasCustomSettings => false;
    public virtual void DrawCustomSettings() { }
    
    public abstract bool NeedsToGenerate();
    protected abstract Dictionary<string, string> GenerateMagicValues(bool forceGenerate);
    
    public void Refresh(bool forceGenerate = false) {
        if (forceGenerate || MagicValueSorcererSettings.IsSorcererEnabled(this)) {
            var magicValues = GenerateMagicValues(forceGenerate);
            CreateFiles(magicValues);
        }
    }
    
    protected void CreateFiles(Dictionary<string, string> magicValues) {
        foreach (var (path, code) in magicValues) {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, code);
        }

        if (magicValues.Any()) {
            AssetDatabase.Refresh();
        }
    }

    public static MagicValueSorcerer[] All { get; } = TypeCache.GetTypesDerivedFrom<MagicValueSorcerer>()
        .Where(type => !type.IsAbstract)
        .Select(Activator.CreateInstance)
        .OfType<MagicValueSorcerer>()
        .OrderBy(sorcerer => sorcerer.DisplayName)
        .ToArray();

    public static bool AnyNeedsToGenerate() {
        return All.Any(sorcerer => MagicValueSorcererSettings.IsSorcererEnabled(sorcerer) && sorcerer.NeedsToGenerate());
    }
}
}