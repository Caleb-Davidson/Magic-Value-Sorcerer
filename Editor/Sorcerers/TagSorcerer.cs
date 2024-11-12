using JetBrains.Annotations;
using UnityEditorInternal;

namespace Magic_Value_Sorcerer.Editor.Sorcerers {
[UsedImplicitly]
public class TagSorcerer : DefaultMagicValueSorcerer {
    public override string ClassName => "Tags";

    protected override string Generate(ClassBuilder builder) {
        foreach (var tag in InternalEditorUtility.tags) {
            builder.AddConst(tag);
        }
        return builder.Build();
    }

    public override bool NeedsToGenerate() => 
        SorcererUtils.AreConstValuesMissing(this, InternalEditorUtility.tags);
}
}