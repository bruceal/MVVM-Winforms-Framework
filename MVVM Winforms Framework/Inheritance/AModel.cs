namespace MVVMWinformsFramework.Inheritance
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Annotations;
    using Binding;

    public abstract class AModel : DynamicObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly Dictionary<String, FieldInfo> fieldsDictionary;
        private readonly Dictionary<String, PropertyInfo> propertiesDictionary;

        protected AModel()
        {
            var modelType = GetType();
            var fields = modelType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            var properties =
                modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

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

            SetUpAutoPropertyChanges();
        }

        #region INotify
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        protected void SetUpAutoPropertyChanges()
        {
            var modelType = GetType();

            foreach (var propertyInfo in modelType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                if (!propertyInfo.PropertyType.Name.Contains("AutoNotifyProperty"))
                    continue;

                if (propertyInfo.GetValue(this) == null)
                {
                    var genericType = propertyInfo.PropertyType.GetGenericArguments().First();
                    var propertyType = typeof(AutoNotifyProperty<>).MakeGenericType(genericType);
                    propertyInfo.SetValue(this, Activator.CreateInstance(propertyType));
                }

                var obj = propertyInfo.GetValue(this) as AAutoNotify;

                if (obj != null)
                {
                    obj.parent = this;
                    obj.updateArgs = new PropertyChangedEventArgs(propertyInfo.Name);
                    obj.PropertyChanged += (Sender, Args) => PropertyChanged?.Invoke(Sender, Args);
                }
            }
        }
        #endregion

        #region DynamicAccess
        public override bool TryGetMember(GetMemberBinder Binder, out Object Result)
        {
            Result = null;
            if (fieldsDictionary.ContainsKey(Binder.Name))
            {
                Result = fieldsDictionary[Binder.Name].GetValue(this);
                return true;
            }

            if (propertiesDictionary.ContainsKey(Binder.Name))
            {
                Result = propertiesDictionary[Binder.Name].GetValue(this);
                return true;
            }

            return false;
        }

        public override bool TrySetMember(SetMemberBinder Binder, Object Value)
        {
            if (fieldsDictionary.ContainsKey(Binder.Name))
            {
                fieldsDictionary[Binder.Name].SetValue(this, Value);
                return true;
            }

            if (propertiesDictionary.ContainsKey(Binder.Name))
            {
                propertiesDictionary[Binder.Name].SetValue(this, Value);
                return true;
            }

            return false;
        }
        #endregion


    }
}