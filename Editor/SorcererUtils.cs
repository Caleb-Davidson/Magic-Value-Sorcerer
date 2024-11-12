using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Magic_Value_Sorcerer.Editor {
public static class SorcererUtils {
    public static string DefaultGeneratedNamespace => "Magic_Value_Sorcerer.Generated";
    public static string DefaultGeneratedAssembly => "Magic_Value_Sorcerer.Generated";
    public static string GetDefaultGenerationDirectory => "Assets/Plugins/Magic Value Sorcerer/Generated/";
    
    public static Type? GetGeneratedType(SimpleMagicValueSorcerer sorcerer) {
        return GetGeneratedType(sorcerer.ClassName, sorcerer.Namespace, sorcerer.Assembly);
    }
    
    public static Type? GetGeneratedType(string className, string namespaceName, string assemblyName) {
        return Type.GetType(namespaceName + "." + className + ", " + assemblyName);
    }

    public static ReadOnlyCollection<string> GetConstValues(SimpleMagicValueSorcerer sorcerer) {
        var generatedType = GetGeneratedType(sorcerer);
        return generatedType == null ? new List<string>().AsReadOnly() : GetConstValues(generatedType);
    }
    
    public static ReadOnlyCollection<string> GetConstValues(string className, string namespaceName, string assemblyName) {
        var generatedType = GetGeneratedType(className, namespaceName, assemblyName);
        return generatedType == null ? new List<string>().AsReadOnly() : GetConstValues(generatedType);
    }

    public static ReadOnlyCollection<string> GetConstValues(Type type) {
        return type.GetFields()
            .Select(field => field.GetValue(null))
            .OfType<string>()
            .ToList()
            .AsReadOnly();
    }
    
    public static ReadOnlyCollection<T> GetStaticValues<T>(SimpleMagicValueSorcerer sorcerer) {
        var generatedType = GetGeneratedType(sorcerer);
        return generatedType == null ? new List<T>().AsReadOnly() : GetStaticValues<T>(generatedType);
    }
    
    public static ReadOnlyCollection<T> GetStaticValues<T>(string className, string namespaceName, string assemblyName) {
        var generatedType = GetGeneratedType(className, namespaceName, assemblyName);
        return generatedType == null ? new List<T>().AsReadOnly() : GetStaticValues<T>(generatedType);
    }
    
    public static ReadOnlyCollection<T> GetStaticValues<T>(Type type) {
        return type.GetProperties()
            .Select(property => property.GetValue(null))
            .OfType<T>()
            .ToList()
            .AsReadOnly();
    }
    
    public static ReadOnlyCollection<string> GetInnerClassesNames(SimpleMagicValueSorcerer sorcerer) {
        var generatedType = GetGeneratedType(sorcerer);
        return generatedType == null ? new List<string>().AsReadOnly() : GetInnerClassesNames(generatedType);
    }
    
    public static ReadOnlyCollection<string> GetInnerClassesNames(string className, string namespaceName, string assemblyName) {
        var generatedType = GetGeneratedType(className, namespaceName, assemblyName);
        return generatedType == null ? new List<string>().AsReadOnly() : GetInnerClassesNames(generatedType);
    }
    
    public static ReadOnlyCollection<string> GetInnerClassesNames(Type type) {
        return type.GetNestedTypes()
            .Select(nestedType => nestedType.Name)
            .ToList()
            .AsReadOnly();
    }

    public static ReadOnlyCollection<Type> GetInnerClasses(SimpleMagicValueSorcerer sorcerer) {
        var generatedType = GetGeneratedType(sorcerer);
        return generatedType == null ? new List<Type>().AsReadOnly() : GetInnerClasses(generatedType);
    }
    
    public static ReadOnlyCollection<Type> GetInnerClasses(string className, string namespaceName, string assemblyName) {
        var generatedType = GetGeneratedType(className, namespaceName, assemblyName);
        return generatedType == null ? new List<Type>().AsReadOnly() : GetInnerClasses(generatedType);
    }
    
    public static ReadOnlyCollection<Type> GetInnerClasses(Type type) {
        return type.GetNestedTypes()
            .ToList()
            .AsReadOnly();
    }

    public static bool AreConstValuesMissing(SimpleMagicValueSorcerer sorcerer, ICollection<string> expectedValues) {
        return AreValuesMissing(expectedValues, GetConstValues(sorcerer));
    }
    
    public static bool AreConstValuesMissing(string className, string namespaceName, string assemblyName, ICollection<string> expectedValues) {
        return AreValuesMissing(expectedValues, GetConstValues(className, namespaceName, assemblyName));
    }
    
    public static bool AreStaticValuesMissing<T>(SimpleMagicValueSorcerer sorcerer, ICollection<T> expectedValues) {
        return AreValuesMissing(expectedValues, GetStaticValues<T>(sorcerer));
    }
    
    public static bool AreStaticValuesMissing<T>(string className, string namespaceName, string assemblyName, ICollection<T> expectedValues) {
        return AreValuesMissing(expectedValues, GetStaticValues<T>(className, namespaceName, assemblyName));
    }

    public static bool AreInnerClassesMissing(SimpleMagicValueSorcerer sorcerer, ICollection<string> expectedInnerClasses) {
        return AreValuesMissing(expectedInnerClasses, GetInnerClassesNames(sorcerer));
    }

    public static bool AreInnerClassesMissing(string className, string namespaceName, string assemblyName, ICollection<string> expectedInnerClasses) {
        return AreValuesMissing(expectedInnerClasses, GetInnerClassesNames(className, namespaceName, assemblyName));
    }

    private static bool AreValuesMissing<T>(ICollection<T> expectedValues, ICollection<T> existingValues) {
        return expectedValues.Count != existingValues.Count || expectedValues.Except(existingValues).Any();
    }
}
}