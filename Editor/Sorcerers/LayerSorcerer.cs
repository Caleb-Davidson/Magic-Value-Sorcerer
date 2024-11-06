using Magic_Value_Sorcerer.Editor;
using Magic_Value_Sorcerer.Editor.Sorcerers;
using UnityEditorInternal;

[assembly: RegisterMagicValueSorcerer(typeof(LayerSorcerer))]

namespace Magic_Value_Sorcerer.Editor.Sorcerers {
public class LayerSorcerer : MagicValueSorcerer{
    public override string ClassName => "Layers";
    
    public override string Generate() {
        var builder = new ClassBuilder(this);
        foreach (var tag in InternalEditorUtility.layers) {
            builder.AddConst(tag);
        }
        return builder.Build();
    }

    public override bool NeedsToGenerate() => 
        SorcererUtils.AreConstValuesMissing(this, InternalEditorUtility.layers);
}
}