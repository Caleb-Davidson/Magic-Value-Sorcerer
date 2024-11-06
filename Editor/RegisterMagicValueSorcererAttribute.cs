using System;

namespace Magic_Value_Sorcerer.Editor {
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class RegisterMagicValueSorcererAttribute : Attribute {
    public Type Type { get; }

    public RegisterMagicValueSorcererAttribute(Type type) {
        Type = type;
    }
}
}