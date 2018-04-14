using System;
using System.Collections.Generic;

namespace MVVMWinformsFramework.Binding
{
    using System.Dynamic;
    using System.Reflection;
    using Inheritance;

    public class ViewDynamicAccess : DynamicObject
    {
        private readonly IBoundView view;
        private readonly Dictionary<String, FieldInfo> fieldsDictionary;
        private readonly Dictionary<String, PropertyInfo> propertiesDictionary;
        private readonly Dictionary<String, MemberInfo> declaredDictionary;

        public ViewDynamicAccess(IBoundView View)
        {
            view = View;
            var viewType = View.GetType();
            var fields = viewType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var properties =
                viewType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var delcaredMembers = viewType.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance |
                                                      BindingFlags.Public | BindingFlags.NonPublic);
            
            fieldsDictionary = new Dictionary<string, FieldInfo>();
            foreach (var fieldInfo in fields)
            {
                fieldsDictionary.Add(fieldInfo.Name, fieldInfo);
            }

            propertiesDictionary = new Dictionary<string, PropertyInfo>();
            foreach (var propertyInfo in properties)
            {
                propertiesDictionary.Add(propertyInfo.Name, propertyInfo);
            }

            declaredDictionary = new Dictionary<string, MemberInfo>();
            foreach (var delcaredMember in delcaredMembers)
            {
                declaredDictionary.Add(delcaredMember.Name, delcaredMember);
            }

        }

        public override bool TryGetMember(GetMemberBinder Binder, out object Result)
        {
            if (fieldsDictionary.ContainsKey(Binder.Name))
            {
                Result = fieldsDictionary[Binder.Name].GetValue(view);
                return true;
            }

            if (propertiesDictionary.ContainsKey(Binder.Name))
            {
                Result = propertiesDictionary[Binder.Name].GetValue(view);
                return true;
            }

            Result = null;
            return false;
        }

        public override bool TrySetMember(SetMemberBinder Binder, object Value)
        {
            if (fieldsDictionary.ContainsKey(Binder.Name))
            {
                fieldsDictionary[Binder.Name].SetValue(view, Value);
                return true;
            }

            if (propertiesDictionary.ContainsKey(Binder.Name))
            {
                propertiesDictionary[Binder.Name].SetValue(view, Value);
                return true;
            }

            return false;
        }

        public bool GetValue(String Name, out Object Result)
        {
            if (fieldsDictionary.ContainsKey(Name))
            {
                Result = fieldsDictionary[Name].GetValue(view);
                return true;
            }

            if (propertiesDictionary.ContainsKey(Name) && propertiesDictionary[Name].CanRead)
            {
                Result = propertiesDictionary[Name].GetValue(view);
                return true;
            }

            if (Name == "this")
            {
                Result = view;
                return true;
            }

            Result = null;
            return false;
        }

        public bool SetValue(String Name, Object Value)
        {
            if (fieldsDictionary.ContainsKey(Name))
            {
                fieldsDictionary[Name].SetValue(view, Value);
                return true;
            }

            if (propertiesDictionary.ContainsKey(Name))
            {
                if (propertiesDictionary[Name].CanWrite)
                {
                    propertiesDictionary[Name].SetValue(view, Value);
                    return true;
                }
            }

            return false;
        }

        public bool BindMethod(String Name, dynamic Property)
        {
            var methodInfo = view.GetType().GetMethod(Name, Property.GetArguments());

            if (methodInfo == null)
                return false;

            Property.Function = methodInfo.CreateDelegate(Property.GetType(), view);
            return true;
        }
    }
}
