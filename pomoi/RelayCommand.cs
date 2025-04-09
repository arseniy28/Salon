using System.Windows.Input;

public class RelayCommand : ICommand
{
    private readonly Func<object, Task> _executeAsync;
    private readonly Func<object, bool> _canExecute;

    public RelayCommand(Func<object, Task> executeAsync, Func<object, bool> canExecute = null)
    {
        _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;

    public async void Execute(object parameter)
    {
        if (CanExecute(parameter))
        {
            await _executeAsync(parameter);
        }
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}