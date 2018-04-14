namespace MVVMWinformsFramework
{
    using System;

    public static class TypeExtension
    {
        public static dynamic GetSetablePublicAttribute(this Type Type, String Name)
        {
            var fieldInfo = Type.GetField(Name);
            if (fieldInfo != null)
                return fieldInfo;

            return Type.GetProperty(Name);
        }
    }
}
