namespace MVVMWinformsFramework.Inheritance
{
    using System;
    using System.ComponentModel;
    using System.Dynamic;
    using Binding;

    public abstract class ABoundProperty : DynamicObject
    {
        public String PropertyName { get; set; }
        public ViewDynamicAccess ViewBinder { get; set; }
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool OneTime { get; set; }

        public delegate void BindFailure(BindAction Action, String PropertyName);

        public static event BindFailure BindFail;

        protected void RaiseBindFailEvent(BindAction Action, String PropertyName)
        {
            BindFail?.Invoke(Action, PropertyName);
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
