﻿using System;
using System.Windows.Input;

namespace Helpers
  {
  /// <summary>
  /// Non Generic Implementation of a RelayCommand
  /// </summary>
  public class CommandBase : ICommand
    {
    #region Private Fields

    private readonly Action _execute;
    private readonly Predicate<object> _canExecute;

    #endregion

    #region Constructor


    /// <summary>
    /// Initializes a new instance of the <see cref="CommandBase"/> class.
    /// </summary>
    /// <param name="execute">The execute action.</param>
    /// <param name="canExecute">The can execute predicate.</param>
    /// <exception cref="NullReferenceException">execute</exception>
    public CommandBase(Action execute, Predicate<object> canExecute = null)
      {
      _execute = execute ?? throw new NullReferenceException(nameof(execute));
      _canExecute = canExecute ?? (_ => true);
      }

    #endregion

    #region Event Handler


    /// <summary>
    /// Occurs when changes occur that affect whether or not the command should execute.
    /// </summary>
    public event EventHandler CanExecuteChanged
      {
      add => CommandManager.RequerySuggested += value;
      remove => CommandManager.RequerySuggested -= value;
      }

    #endregion

    #region Methods


    /// <summary>
    /// Defines the method that determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
    /// <returns><see langword="true" /> if this command can be executed; otherwise, <see langword="false" />.</returns>
    public bool CanExecute(object parameter = null) => _canExecute(parameter);


    /// <summary>
    /// Defines the method to be called when the command is invoked.
    /// </summary>
    /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
    public void Execute(object parameter) => _execute();

    #endregion

    }
  }
