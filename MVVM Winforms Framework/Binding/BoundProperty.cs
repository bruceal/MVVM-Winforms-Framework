using System;

namespace MVVMWinformsFramework.Binding
{
    using System.Dynamic;
    using Inheritance;

    public class BoundProperty<T> : ABoundProperty
    {
        private Object value;

        public T GetValue()
        {
            T result = default(T);
            bool success = false;
            if (ViewBinder != null && !String.IsNullOrWhiteSpace(PropertyName) && CanRead)
            {
                if (OneTime)
                {
                    success = true;
                    if (CanRead)
                        success = ViewBinder.GetValue(PropertyName, out value);

                    if (success)
                        result = (T)value;

                }
                else
                {
                    Object value;
                    success = ViewBinder.GetValue(PropertyName, out value);
                    if (success)
                        result = (T)value;
                }
            }

            if (!success)
            {
                RaiseBindFailEvent(BindAction.Get, PropertyName);
            }


            return result;
        }

        public override bool TryGetMember(GetMemberBinder Binder, out Object Result)
        {
            bool success = false;
            Result = null;

            String memberName = Binder.Name == "Value" && !String.IsNullOrWhiteSpace(PropertyName)
                ? PropertyName
                : Binder.Name; 

            if (ViewBinder != null && !String.IsNullOrWhiteSpace(memberName) && CanRead)
            {
                if (OneTime)
                {
                    success = true;
                    Result = null;
                    if (CanRead)
                        success = ViewBinder.GetValue(memberName, out value);

                    if (success)
                        Result = value;

                }
                else
                {
                    success = ViewBinder.GetValue(memberName, out Result);
                }
            }

            if (!success)
            {
                RaiseBindFailEvent(BindAction.Get, memberName);
            }


            return true;
        }

        public void SetValue(T Value)
        {
            bool success = false;
            if (ViewBinder != null && !String.IsNullOrWhiteSpace(PropertyName) && CanWrite)
            {
                success = ViewBinder.SetValue(PropertyName, Value);
            }

            if (!success)
            {
                RaiseBindFailEvent(BindAction.Set, PropertyName);
            }
        }

        public override bool TrySetMember(SetMemberBinder Binder, Object Value)
        {
            bool success = false;

            String memberName = Binder.Name == "Value" && !String.IsNullOrWhiteSpace(PropertyName)
                ? PropertyName
                : Binder.Name;

            if (ViewBinder != null && !String.IsNullOrWhiteSpace(memberName) && CanWrite)
            {
                success = ViewBinder.SetValue(memberName, Value);
            }

            if (!success)
            {
                RaiseBindFailEvent(BindAction.Set, memberName);
            }

            return true;
        }

        public Type GetType()
        {
            return typeof(T);
        }
    }
}
