using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace RisenautEditor.ViewModel
{
    /// <summary>
    /// A command that sets focus to its parameter.
    /// </summary>
    class SetFocusCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            UIElement el = parameter as UIElement;
            if (el != null)
            {
                el.Focus();
            }
        }
    }
}
