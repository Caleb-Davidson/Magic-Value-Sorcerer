using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Magic_Value_Sorcerer.Editor {
public static class SorcererUtils {
    public static string DefaultGeneratedNamespace => "Magic_Value_Sorcerer.Generated";
    public static string DefaultGeneratedAssembly => "Magic_Value_Sorcerer.Generated";
    public static string GetDefaultGenerationDirectory => "Assets/Plugins/Magic Value Sorcerer/Generated/";
    
    public static Type? GetGeneratedType(MagicValueSorcerer sorcerer) {
        return Type.GetType(sorcerer.Namespace + "." + sorcerer.ClassName + ", " + sorcerer.Assembly);
    }

    public static ReadOnlyCollection<string> GetConstValues(MagicValueSorcerer sorcerer) {
        var generatedType = GetGeneratedType(sorcerer);
        return generatedType == null ? new List<string>().AsReadOnly() : GetConstValues(generatedType);
    }

    public static ReadOnlyCollection<string> GetConstValues(Type type) {
        return type.GetFields()
            .Select(field => (string)field.GetValue(null))
            .ToList()
            .AsReadOnly();
    }
    
    public static ReadOnlyCollection<T> GetStaticValues<T>(MagicValueSorcerer sorcerer) {
        var generatedType = GetGeneratedType(sorcerer);
        return generatedType == null ? new List<T>().AsReadOnly() : GetStaticValues<T>(generatedType);
    }
    
    public static ReadOnlyCollection<T> GetStaticValues<T>(Type type) {
        return type.GetProperties()
            .Select(property => (T)property.GetValue(null))
            .ToList()
            .AsReadOnly();
    }
    
    public static ReadOnlyCollection<string> GetInnerClassesNames(MagicValueSorcerer sorcerer) {
        var generatedType = GetGeneratedType(sorcerer);
        return generatedType == null ? new List<string>().AsReadOnly() : GetInnerClassesNames(generatedType);
    }
    
    public static ReadOnlyCollection<string> GetInnerClassesNames(Type type) {
        return type.GetNestedTypes()
            .Select(nestedType => nestedType.Name)
            .ToList()
            .AsReadOnly();
    }

    public static ReadOnlyCollection<Type> GetInnerClasses(MagicValueSorcerer sorcerer) {
        var generatedType = GetGeneratedType(sorcerer);
        return generatedType == null ? new List<Type>().AsReadOnly() : GetInnerClasses(generatedType);
    }
    
    public static ReadOnlyCollection<Type> GetInnerClasses(Type type) {
        return type.GetNestedTypes()
            .ToList()
            .AsReadOnly();
    }

    public static bool AreConstValuesMissing(MagicValueSorcerer sorcerer, ICollection<string> expectedValues) {
        var existingValues = GetConstValues(sorcerer);
        return expectedValues.Count != existingValues.Count || expectedValues.Except(existingValues).Any();
    }
    
    public static bool AreStaticValuesMissing<T>(MagicValueSorcerer sorcerer, ICollection<T> expectedValues) {
        var existingValues = GetStaticValues<T>(sorcerer);
        return expectedValues.Count != existingValues.Count || expectedValues.Except(existingValues).Any();
    }

    public static bool AreInnerClassesMissing(MagicValueSorcerer sorcerer, ICollection<string> expectedInnerClasses) {
        var existingInnerClasses = GetInnerClassesNames(sorcerer);
        return expectedInnerClasses.Count != existingInnerClasses.Count || expectedInnerClasses.Except(existingInnerClasses).Any();
    }
}
}