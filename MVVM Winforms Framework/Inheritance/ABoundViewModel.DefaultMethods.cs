namespace MVVMWinformsFramework.Inheritance
{
    using System;
    using System.Windows.Forms;

    public abstract partial class ABoundViewModel
    {
        public bool InvokeRequired()
        {
            return InvokeRequiredBinding.InvokeRequired;
        }

        public dynamic Invoke(Delegate Action)
        {
            return InvokeBinding.Invoke(new Object[] { Action });
        }

        public bool Visible
        {
            get { return VisibleBinding.Visible; }
            set
            {
                if (InvokeRequired())
                {
                    Invoke(new Action(() => Visible = value));
                }
                else
                {
                    VisibleBinding.Visible = value;
                }
            }
        }
        public void Show()
        {
            if (InvokeRequired())
            {
                Invoke(new Action(Show));
            }
            else
            {
                ShowBinding.Invoke(new Object[0]);
            }
        }
        public void Show(Control Owner)
        {
            if (InvokeRequired())
            {
                Invoke(new Action(() => Show(Owner)));
            }
            else
            {
                ShowWithOwnerBinding.Invoke(new[] { Owner });
            }
        }
        public void ShowViewModelWithOwner(Inheritance.ABoundViewModel ViewToShow)
        {
            ViewToShow.Show(view as Control);
        }
        public DialogResult ShowViewModelAsDialogFromOwner(Inheritance.ABoundViewModel ViewToShow)
        {
            return ViewToShow.ShowDialog((view as Control)?.TopLevelControl);
        }
        public DialogResult ShowDialog(Control Owner)
        {
            return InvokeRequired()
                ? (DialogResult)Invoke(new Func<DialogResult>(() => ShowDialog(Owner)))
                : (DialogResult)ShowDialogBinding.Invoke(new Object[] { Owner });
        }

        public bool IsAddedToControl(Control Control)
        {
            var controlToCheck = view as Control;
            if (controlToCheck != null)
            {
                return Control.Controls.Contains(controlToCheck);
            }

            return false;
        }
        public void AddToControl(Control Control, DockStyle Dock)
        {
            var controlToAdd = view as Control;
            if (controlToAdd != null)
            {
                Control.Controls.Add(controlToAdd);
                controlToAdd.Dock = Dock;
            }
        }

        public void Close()
        {
            if (InvokeRequired())
            {
                Invoke(new Action(Close));
            }
            else
            {
                CloseBinding.Invoke(new Object[0]);
            }
        }

    }
}
