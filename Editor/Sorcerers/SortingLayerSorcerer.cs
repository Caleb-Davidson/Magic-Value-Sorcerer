using System.Reflection;
using JetBrains.Annotations;
using UnityEditorInternal;

namespace Magic_Value_Sorcerer.Editor.Sorcerers {
[UsedImplicitly]
public class SortingLayerSorcerer : DefaultMagicValueSorcerer {
    public override string ClassName => "SortingLayers";

    protected override string Generate(ClassBuilder builder) {
        foreach (var tag in GetSortingLayers()) {
            builder.AddConst(tag);
        }
        return builder.Build();
    }

    public override bool NeedsToGenerate() => 
        SorcererUtils.AreConstValuesMissing(this, GetSortingLayers());

    private static string[] GetSortingLayers() {
        return (string[])typeof(InternalEditorUtility)
            .GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic)!
            .GetValue(null);
    }
}
}