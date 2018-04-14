namespace MVVMWinformsFramework.Inheritance
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using Binding;

    public abstract partial class ABoundViewModel
    {
        #region Data
        [DataBinder("InvokeRequired", DataBindType.OneWay, typeof(bool))]
        protected dynamic InvokeRequiredBinding { get; set; }
        [DataBinder("Visible", DataBindType.TwoWay, typeof(bool))]
        protected dynamic VisibleBinding { get; set; }
        [DataBinder("DialogResult", DataBindType.TwoWay, typeof(DialogResult))]
        protected dynamic DialogResultBinding { get; set; }

        #endregion

        #region Methods
        [MethodBinder("Invoke", typeof(Func<Delegate, Object>))]
        protected dynamic InvokeBinding { get; set; }
        [MethodBinder("Show", typeof(Action))]
        protected dynamic ShowBinding { get; set; }
        [MethodBinder("Show", typeof(Action<Control>))]
        protected dynamic ShowWithOwnerBinding { get; set; }
        [MethodBinder("ShowDialog", typeof(Func<Form, DialogResult>))]
        protected dynamic ShowDialogBinding { get; set; }
        [MethodBinder("Close", typeof(Action))]
        protected dynamic CloseBinding { get; set; }
        [MethodBinder("PointToScreen", typeof(Func<Point, Point>))]
        protected dynamic PointToScreenBinding { get; set; }

        #endregion

        #region Events
        protected event Action FormClosing;
        [EventBinder("this", "FormClosing")]
        protected void Closing(Object Sender, FormClosingEventArgs e)
        {
            FormClosing?.Invoke();
        }

        #endregion
    }
}
