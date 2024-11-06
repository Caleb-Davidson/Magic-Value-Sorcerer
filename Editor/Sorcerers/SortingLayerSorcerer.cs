using System.Reflection;
using Magic_Value_Sorcerer.Editor;
using Magic_Value_Sorcerer.Editor.Sorcerers;
using UnityEditorInternal;

[assembly: RegisterMagicValueSorcerer(typeof(SortingLayerSorcerer))]

namespace Magic_Value_Sorcerer.Editor.Sorcerers {
public class SortingLayerSorcerer : MagicValueSorcerer{
    public override string ClassName => "SortingLayers";
    
    public override string Generate() {
        var builder = new ClassBuilder(this);
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