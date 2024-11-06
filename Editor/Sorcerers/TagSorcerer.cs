using Magic_Value_Sorcerer.Editor;
using Magic_Value_Sorcerer.Editor.Sorcerers;
using UnityEditorInternal;

[assembly: RegisterMagicValueSorcerer(typeof(TagSorcerer))]

namespace Magic_Value_Sorcerer.Editor.Sorcerers {
public class TagSorcerer : MagicValueSorcerer {
    public override string ClassName => "Tags";
    
    public override string Generate() {
        var builder = new ClassBuilder(this);
        foreach (var tag in InternalEditorUtility.tags) {
            builder.AddConst(tag);
        }
        return builder.Build();
    }

    public override bool NeedsToGenerate() => 
        SorcererUtils.AreConstValuesMissing(this, InternalEditorUtility.tags);
}
}