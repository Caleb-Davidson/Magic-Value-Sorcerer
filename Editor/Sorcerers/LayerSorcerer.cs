using JetBrains.Annotations;
using UnityEditorInternal;

namespace Magic_Value_Sorcerer.Editor.Sorcerers {
[UsedImplicitly]
public class LayerSorcerer : DefaultMagicValueSorcerer {
    public override string ClassName => "Layers";

    protected override string Generate(ClassBuilder builder) {
        foreach (var tag in InternalEditorUtility.layers) {
            builder.AddConst(tag);
        }
        return builder.Build();
    }

    public override bool NeedsToGenerate() => 
        SorcererUtils.AreConstValuesMissing(this, InternalEditorUtility.layers);
}
}