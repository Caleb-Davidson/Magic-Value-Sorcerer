using System.IO;
using UnityEditor;

namespace Magic_Value_Sorcerer.Editor {
public static class MagicValueGenerator {
    public static void GenerateAll(bool forceGenerate = false) {
        foreach (var sorcerer in MagicValueSorcerer.All) {
            Generate(sorcerer, forceGenerate);
        }
    }

    public static void Generate(MagicValueSorcerer sorcerer, bool forceGenerate = false) {
        if (forceGenerate || (MagicValueSorcererSettings.IsSorcererEnabled(sorcerer) && sorcerer.NeedsToGenerate())) {
            var code = sorcerer.Generate();
            File.WriteAllText(Path.Combine(sorcerer.Directory, sorcerer.ClassName + ".cs"), code);
        }
        AssetDatabase.Refresh();
    }
}
}