using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Galaga.Command
{
    /// <summary>
    /// relay command class
    /// </summary>
    /// <seealso cref="System.Windows.Input.ICommand" />
    public class RelayCommand : ICommand
    {
        #region Data Members

        private readonly Action execute;
        private readonly Func<bool> canExecute;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The execute.</param>
        /// <param name="canExecute">The can execute.</param>
        /// <exception cref="System.ArgumentNullException">execute</exception>
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute();
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            this.execute();
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        /// <returns></returns>
        public event EventHandler CanExecuteChanged;

        #endregion
    }
}
