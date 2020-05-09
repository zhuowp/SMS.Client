using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.MVVM
{
    public class RelayCommand : GalaSoft.MvvmLight.Command.RelayCommand
    {
        public RelayCommand(Action execute, bool keepTargetAlive = false) : base(execute, keepTargetAlive)
        { }

        public RelayCommand(Action execute, Func<bool> canExecute, bool keepTargetAlive = false) : base(execute, canExecute, keepTargetAlive)
        { }
    }

    public class RelayCommand<T> : GalaSoft.MvvmLight.Command.RelayCommand<T>
    {
        public RelayCommand(Action<T> execute, bool keepTargetAlive = false) : base(execute, keepTargetAlive)
        { }

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute, bool keepTargetAlive = false) : base(execute, canExecute, keepTargetAlive)
        { }
    }
}
