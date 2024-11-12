using System.Collections.Generic;
using System.IO;

namespace Magic_Value_Sorcerer.Editor {
public abstract class SimpleMagicValueSorcerer : MagicValueSorcerer {
    public override string DisplayName => ClassName;
    public abstract string ClassName { get; }
    public abstract string Namespace { get; }
    public abstract string Assembly { get; }
    protected abstract string Directory { get; }
    
    protected abstract string Generate(ClassBuilder builder);

    protected override Dictionary<string, string> GenerateMagicValues(bool forceGenerate) {
        if (!forceGenerate && !NeedsToGenerate()) return new Dictionary<string, string>();
        return new Dictionary<string, string> {
            {
                Path.Combine(Directory, ClassName + ".cs"),
                Generate(new ClassBuilder(GetType().FullName!, ClassName, Namespace))
            }
        };
    }
}
}