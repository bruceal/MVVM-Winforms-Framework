
namespace MVVMWinformsFramework.Factories
{
    using System;
    using Inheritance;

    public abstract class ModelFactory
    {
        public abstract AModel GetModel();

        public delegate ModelFactory GetFactoryDelegate(Type Type);

        public static event GetFactoryDelegate GetFactoryRequest;

        public static ModelFactory GetFactory(Type TypeToGenerate)
        {
            var result = GetFactoryRequest?.Invoke(TypeToGenerate);
            if (result != null)
                return result;


            var generatorType = typeof(GenericModelFactory<>).MakeGenericType(TypeToGenerate);
            return (ModelFactory)Activator.CreateInstance(generatorType);
        }
    }

    public class GenericModelFactory<T> : ModelFactory
    {
        public override AModel GetModel()
        {
            return (AModel)Activator.CreateInstance(typeof(T));
        }
    }
}
