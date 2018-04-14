
namespace MVVMWinformsFramework.Binding
{
    using System;
    using System.ComponentModel;
    using System.Dynamic;
    using System.Reflection;
    using ProtoBuf;

    public abstract class AAutoNotify: DynamicObject
    {
        public abstract event PropertyChangedEventHandler PropertyChanged;
        public PropertyChangedEventArgs updateArgs;
        public object parent;
    }

    [ProtoContract]
    public class AutoNotifyProperty<T> : AAutoNotify
    {
        [ProtoMember(1)]
        private T value;

        private readonly FieldInfo valueInfo;

        public AutoNotifyProperty()
        {
            valueInfo = GetType().GetField("value", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public void SetValue(T Value)
        {
            value = Value;
            Updated();
        }

        public T GetValue()
        {
            return value;
        }

        public void Updated()
        {
            PropertyChanged?.Invoke(parent, updateArgs);
        }

        public override event PropertyChangedEventHandler PropertyChanged;

        #region DynamicAccess
        public override bool TryGetMember(GetMemberBinder Binder, out Object Result)
        {
            Result = null;
            if (Binder.Name == "Value" && valueInfo != null)
            {
                Result = valueInfo.GetValue(this);
                return true;
            }

            return false;
        }

        public override bool TrySetMember(SetMemberBinder Binder, Object Value)
        {
            if (Binder.Name == "Value" && valueInfo != null)
            {
                valueInfo.SetValue(this, Value);
                Updated();
                return true;
            }

            return false;
        }
        #endregion

    }
}
