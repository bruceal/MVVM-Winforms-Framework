using System;

namespace MVVMWinformsFramework.Binding
{
    public class BoundMethod<T>
    {
        public String PropertyName;
        public Delegate Function;

        public dynamic Invoke(Object[] Arguments)
        {
            return Function.DynamicInvoke(Arguments);
        }
        public Type GetType()
        {
            return typeof(T);
        }

        public Type[] GetArguments()
        {
            var allArguments = GetType().GenericTypeArguments;

            if (IsAction())
                return allArguments;

            if (IsFunction() && allArguments.Length > 1)
            {
                Type[] result = new Type[allArguments.Length - 1];
                Array.Copy(allArguments, 0, result, 0, allArguments.Length - 1);
                return result;
            }

            return new Type[0];
        }

        private bool IsAction()
        {
            return GetType().FullName.StartsWith("System.Action");
        }



        private bool IsFunction()
        {
            return GetType().FullName.StartsWith("System.Func");
        }
    }
}
