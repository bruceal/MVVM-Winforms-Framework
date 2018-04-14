using System;
namespace MVVMWinformsFramework.Binding
{
    public class EventBinder : Attribute
    {
        public EventBinder(String PropertyName, BindableEvent Event)
        {
            PropertyToBind = PropertyName;
            EventToBind = Event;
        }

        public EventBinder(String PropertyName, String Event)
        {
            PropertyToBind = PropertyName;
            EventToBindName = Event;
        }
        public String PropertyToBind { get; set; }
        public BindableEvent EventToBind { get; set; }

        public String EventToBindName { get; set; }
    }

    public class DataBinder : Attribute
    {
        public DataBinder(String PropertyName, DataBindType BindingType, Type GenericType = null)
        {
            Type = BindingType;
            PropertyToBind = PropertyName;
            this.GenericType = GenericType;
        }
        public DataBindType Type { get; set; }
        public String PropertyToBind { get; set; }

        public Type GenericType { get; set; }
    }

    public class MethodBinder : Attribute
    {
        public MethodBinder(String MethodName, Type MethodType = null)
        {
            MethodToBind = MethodName;
            GenericType = MethodType;
        }
        public String MethodToBind { get; set; }
        public Type GenericType { get; set; }
    }

    public class ClassBindingAttribute : Attribute
    {
        public ClassBindingAttribute(Type ViewType, Type ModelType)
        {
            this.ViewType = ViewType;
            this.ModelType = ModelType;
        }

        public Type ModelType { get; set; }
        public Type ViewType { get; set; }
    }

    public enum BindableEvent
    {
        Click,
        LinkClicked,
        SelectedIndexChanged,
        EditValueChanged,
        RowCellClick
    }

    public enum DataBindType
    {
        OneTime,
        OneWay,
        TwoWay,
        WriteOnly
    }
}
