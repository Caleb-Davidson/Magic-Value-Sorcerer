namespace Magic_Value_Sorcerer.Editor.Sorcerers {
public abstract class DefaultMagicValueSorcerer : SimpleMagicValueSorcerer {
    public override string Namespace => SorcererUtils.DefaultGeneratedNamespace;
    public override string Assembly => SorcererUtils.DefaultGeneratedAssembly;
    protected override string Directory => SorcererUtils.GetDefaultGenerationDirectory;
}
}