namespace MVVMWinformsFramework.Inheritance
{
    using Binding;

    public interface IBoundView
    {
        ViewDynamicAccess Binder { get; set; }
        Inheritance.ABoundViewModel GetViewModel();
    }
}