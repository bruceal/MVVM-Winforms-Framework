
namespace MVVMWinformsFramework.Factories
{
    using System;
    using Inheritance;
    using ABoundViewModel = Inheritance.ABoundViewModel;

    public abstract class ViewFactory
    {
        public abstract ABoundViewModel GetView();

        public delegate ViewFactory GetFactoryDelegate(Type Type);

        public static event GetFactoryDelegate GetFactoryRequest;

        /*public static ViewFactory GetFactory(String Name)
        {
            return GetFactoryRequest?.Invoke(Name);
        }*/

        public static ViewFactory GetFactory(Type TypeToGenerate)
        {
            var result = GetFactoryRequest?.Invoke(TypeToGenerate);
            if (result != null)
                return result;


            var generatorType = typeof(GenericViewFactory<>).MakeGenericType(TypeToGenerate);
            return (ViewFactory)Activator.CreateInstance(generatorType);
        }
    }

    public class GenericViewFactory<T> : ViewFactory
    {
        public override ABoundViewModel GetView()
        {
            var result = (IBoundView) Activator.CreateInstance(typeof(T));
            return result.GetViewModel();
        }
    }

    
}
