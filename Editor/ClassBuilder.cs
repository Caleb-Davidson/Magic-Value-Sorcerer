﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Magic_Value_Sorcerer.Editor {
public class ClassBuilder {
    private string sourceName;
    private string className;
    private string @namespace;
    private List<string> usings = new();
    private List<string> fields = new();
    private List<string> methods = new();
    private List<ClassBuilder> innerClasses = new();
    
    public ClassBuilder(string sourceName, string className, string @namespace) {
        this.sourceName = sourceName;
        this.className = CleanName(className.Replace(" ", ""));
        this.@namespace = @namespace;
    }
    
    private ClassBuilder(string className) {
        this.className = CleanName(className.Replace(" ", ""));
        @namespace = sourceName = "";
    }
    
    public ClassBuilder AddUsing(string @using) {
        if (!@using.StartsWith("using ")) {
            @using = "using " + @using;
        }
        if (!@using.EndsWith(";")) {
            @using += ";";
        }
        usings.Add(@using);
        return this;
    }
    
    public ClassBuilder AddConst(string nameValue) {
        return AddConstString(nameValue, nameValue);
    }
    
    public ClassBuilder AddConstString(string name, string value) {
        return AddConstField("string", name, $"\"{value}\"");
    }
    
    public ClassBuilder AddConstField(string type, string name, string value) {
        fields.Add($"public const {type} {CleanConstantName(name)} = {value};");
        return this;
    }
    
    public ClassBuilder AddStaticExpressionField(string type, string name, string value) {
        fields.Add($"public static {type} {CleanName(name)} => {value};");
        return this;
    }
    
    public ClassBuilder AddStaticGetterField(string type, string name, string body) {
        fields.Add($"public static {type} {CleanName(name)} {{ get; }} = {body};");
        return this;
    }
    
    public ClassBuilder AddBlankFieldLine() {
        fields.Add("");
        return this;
    }

    public ClassBuilder AddMethod(string methodName, string body, string parameters = "") {
        return AddMethod("void", methodName, body, parameters);
    }
    
    public ClassBuilder AddMethod(string returnType, string methodName, string body, string parameters) {
        // make sure that all lines in the body are indented by 4 spaces
        body = string.Join('\n', body.Split('\n').Select(line => "    " + line));
        methods.Add(string.Format(METHOD_TEMPLATE, returnType, CleanName(methodName), parameters, body));
        return this;
    }
    
    public ClassBuilder CreateInnerClassBuilder(string innerClassName) {
        var innerClass = new ClassBuilder(innerClassName);
        innerClasses.Add(innerClass);
        return innerClass;
    }

    public string Build() {
        return string.Format(CLASS_TEMPLATE,
            sourceName,
            DateTime.Now.ToString("dd MMMM yyyy"),
            GenerateUsings(),
            @namespace,
            className,
            GenerateCode(1)
        ).Replace("\r\n", "\n");
    }
    
    private string GenerateUsings() {
        if (usings.Count == 0) return "";
        return string.Join("\n", usings) + "\n";
    }

    private string GenerateCode(int indentLevel) {
        var code = "";
        var indent = new string(' ', indentLevel * 4);
        
        if (fields.Count > 0) {
            code += string.Join('\n', fields.Select(field => indent + field));
        }
        
        if (methods.Count > 0) {
            if (code != "") {
                code += "\n\n";
            }
            code += string.Join("\n\n", methods.Select(method => string.Join('\n',method.Split('\n').Select(line => indent + line))));
        }
        
        if (innerClasses.Count > 0) {
            if (code != "") {
                code += "\n\n";
            }
            code += string.Join("\n\n", innerClasses.Select(innerClass => innerClass.GenerateInnerClass(indentLevel)));
        }

        return code;
    }
    
    private string GenerateInnerClass(int indentLevel) {
        return string.Format(INNER_CLASS_TEMPLATE,
            new string(' ', indentLevel * 4),
            className,
            GenerateCode(indentLevel + 1)
        );
    }
    
    public static string CleanName(string name, bool removeWhitespace = true, bool toSnakeCase = false) {
        var letters = name.ToList();
        
        if (!char.IsLetter(letters[0])) {
            letters[0] = '_';
        }
        
        for (var i = 1; i < letters.Count; i++) {
            if (removeWhitespace && char.IsWhiteSpace(letters[i])) {
                letters.RemoveAt(i);
                i--;
                continue;
            }
            
            if (!char.IsLetterOrDigit(letters[i])) {
                letters[i] = '_';
                continue;
            }
            
            if (toSnakeCase && char.IsUpper(letters[i]) && letters[i - 1] != '_') {
                letters.Insert(i, '_');
                i++;
                continue;
            }
        }
        
        return new string(letters.ToArray());
    }
    
    public static string CleanConstantName(string name) {
        return CleanName(name, false, true).ToUpperInvariant();
    }

    private const string CLASS_TEMPLATE = @"/* ------------------------------------------------------------------------------
This code was auto-generated by {0} on {1}
Changes to this file may cause incorrect behavior and will be lost if the code is regenerated
------------------------------------------------------------------------------ */
{2}
namespace {3} {{
public static class {4} {{
{5}
}}
}}
";

    private const string INNER_CLASS_TEMPLATE = "{0}public static class {1} {{\n{2}\n{0}}}";
    private const string METHOD_TEMPLATE = "public static {0} {1}({2}) {{\n{3}\n}}";
}
}