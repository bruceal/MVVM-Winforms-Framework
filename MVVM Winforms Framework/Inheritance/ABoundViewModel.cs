namespace MVVMWinformsFramework.Inheritance
{
    using System;
    using System.ComponentModel;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Binding;
    using Factories;
    using Microsoft.CSharp.RuntimeBinder;

    public abstract partial class ABoundViewModel : DynamicObject
    {
        protected AModel Model;
        private readonly IBoundView view;
        protected dynamic ViewAccess => this;
        

        protected ABoundViewModel(IBoundView View)
        {
            view = View;
            Bind(View);

            var classBinding = GetType().GetCustomAttributes().FirstOrDefault(a => a is ClassBindingAttribute);
            var attribute = classBinding as ClassBindingAttribute;
            if (attribute != null)
            {
                SetModel(ModelFactory.GetFactory(attribute.ModelType).GetModel());
            }
        }

        public void SetModel(AModel ModelToSet)
        {
            Model = ModelToSet;

            if (Model == null)
                return;

            Model.PropertyChanged += ModelUpdated;
        }

        public abstract void ModelUpdated(Object Sender, PropertyChangedEventArgs Args);


        #region Binding
        public static ViewModelAndModelPair GetViewModelAndModel(Type ViewModelType)
        {
            var classBinding = ViewModelType.GetCustomAttributes().FirstOrDefault(a=>a is ClassBindingAttribute);

            var attribute = classBinding as ClassBindingAttribute;
            if (attribute != null)
            {
                var viewModel = ViewFactory.GetFactory(attribute.ViewType).GetView();

                return new ViewModelAndModelPair()
                {
                    ViewModel = viewModel,
                    Model = viewModel.Model
                };
            }

            return null;
        }

        public class ViewModelAndModelPair
        {
            public dynamic ViewModel { get; set; }
            public dynamic Model { get; set; }
        }

        public void Bind(IBoundView View)
        {
            View.Binder = new ViewDynamicAccess(View);

            var viewModelType = GetType();

            foreach (var memberInfo in viewModelType.GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
            {
                foreach (Attribute attribute in memberInfo.GetCustomAttributes(false))
                {
                    var type = attribute.GetType();
                    if (type == typeof(EventBinder))
                    {
                        var eventBinder = attribute as EventBinder;
                        if (eventBinder != null)
                        {
                            dynamic propertyForEvent;
                            if (!View.Binder.GetValue(eventBinder.PropertyToBind, out propertyForEvent))
                                continue;

                            var eventInfo = propertyForEvent.GetType()
                                .GetEvent(eventBinder.EventToBindName ?? eventBinder.EventToBind.ToString());
                            if (eventInfo == null)
                                continue;

                            var addHandler = eventInfo.GetAddMethod();
                            var methodInfo = viewModelType.GetMethod(memberInfo.Name,
                                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                            if (methodInfo == null)
                                continue;
                            var @delegate = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, methodInfo);
                            Object[] addHandlerArgs = {@delegate};
                            addHandler.Invoke(propertyForEvent, addHandlerArgs);
                        }
                    } 
                }

            }


            foreach (var propertyInfo in viewModelType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                foreach (Attribute attribute in propertyInfo.GetCustomAttributes(false))
                {
                    var type = attribute.GetType();
                    var property = propertyInfo.GetValue(this);

                    if (type == typeof(DataBinder))
                    {
                        if(!(propertyInfo.PropertyType.Name.Contains("BoundProperty")|| propertyInfo.PropertyType == typeof(Object)))
                            continue;

                        var dataBinder = attribute as DataBinder;
                        if(dataBinder == null)
                            continue;

                        if (property == null)
                        {
                            if(propertyInfo.PropertyType == typeof(Object) && dataBinder.GenericType == null)
                                continue;

                            var genericType = dataBinder.GenericType == null
                                ? propertyInfo.PropertyType.GetGenericArguments().First()
                                : dataBinder.GenericType;

                            var propertyType = typeof(BoundProperty<>).MakeGenericType(genericType);
                            property = Activator.CreateInstance(propertyType);
                            propertyInfo.SetValue(this, property);
                        }

                        var boundProperty = property as ABoundProperty;
                        
                        if (boundProperty != null)
                        {
                            boundProperty.ViewBinder = View.Binder;
                            boundProperty.PropertyName = dataBinder.PropertyToBind;
                            boundProperty.CanRead = CanRead(dataBinder);
                            boundProperty.CanWrite = CanWrite(dataBinder);
                            boundProperty.OneTime = dataBinder.Type == DataBindType.OneTime;
                        }
                    }
                    else if (type == typeof(MethodBinder))
                    {
                        var methodBinder = attribute as MethodBinder;
                        if (methodBinder != null)
                        {
                            if (property == null)
                            {
                                if (!(propertyInfo.PropertyType.Name.Contains("BoundMethod") || propertyInfo.PropertyType == typeof(Object)))
                                    continue;

                                var genericType = methodBinder.GenericType == null
                                    ? propertyInfo.PropertyType.GetGenericArguments().First()
                                    : methodBinder.GenericType;

                                var propertyType = typeof(BoundMethod<>).MakeGenericType(genericType);
                                property = Activator.CreateInstance(propertyType);
                                propertyInfo.SetValue(this, property);
                            }
                            View.Binder.BindMethod(methodBinder.MethodToBind, property);
                        }
                    }
                }

            }
        }

        private bool CanRead(DataBinder DataBinder)
        {
            if (DataBinder == null)
                return false;
            if (DataBinder.Type == DataBindType.WriteOnly)
                return false;

            return true;
        }
        private bool CanWrite(DataBinder DataBinder)
        {
            if (DataBinder == null)
                return false;
            if (DataBinder.Type == DataBindType.WriteOnly || DataBinder.Type == DataBindType.TwoWay)
                return true;

            return false;
        }
        #endregion

        #region DynamicAccess

        public override bool TryGetMember(GetMemberBinder Binder, out object Result)
        {
            return view.Binder.TryGetMember(Binder, out Result);
        }

        public override bool TrySetMember(SetMemberBinder Binder, object Value)
        {
            return view.Binder.TrySetMember(Binder, Value);
        }

        static object GetDynamicMember(object obj, string memberName)
        {
            var binder = Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, memberName, obj.GetType(),
                new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
            var callsite = CallSite<Func<CallSite, object, object>>.Create(binder);
            return callsite.Target(callsite, obj);
        }

        #endregion
    }
}