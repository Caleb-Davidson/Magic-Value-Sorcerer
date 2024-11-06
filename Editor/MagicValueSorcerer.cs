using System;
using System.Linq;
using System.Reflection;

namespace Magic_Value_Sorcerer.Editor {
public abstract class MagicValueSorcerer {
    public abstract string ClassName { get; }
    public virtual string Namespace => SorcererUtils.DefaultGeneratedNamespace;
    public virtual string Assembly => SorcererUtils.DefaultGeneratedAssembly;
    public virtual string Directory => SorcererUtils.GetDefaultGenerationDirectory;
    
    public abstract string Generate();
    public abstract bool NeedsToGenerate();
    
    public virtual bool HasCustomSettings => false;
    public virtual void DrawCustomSettings() { }

    public static MagicValueSorcerer[] All { get; } = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(assembly => assembly.GetCustomAttributes<RegisterMagicValueSorcererAttribute>())
        .Select(attribute => (Activator.CreateInstance(attribute.Type) as MagicValueSorcerer)!)
        .Where(sorcerer => sorcerer != null)
        .OrderBy(sorcerer => sorcerer.ClassName)
        .ToArray();
}
}