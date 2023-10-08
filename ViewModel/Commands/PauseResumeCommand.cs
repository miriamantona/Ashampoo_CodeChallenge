using System;
using System.Windows.Input;

namespace CodeChallengeApp.ViewModel.Commands
{
  public class PauseResumeCommand : ICommand
  {
    private readonly SearchViewModel m_ViewModel;

    public PauseResumeCommand(SearchViewModel viewModel)
    {
      m_ViewModel = viewModel;
    }

    /// <summary>
    /// Whether this command can be executed.
    /// </summary>
    public bool CanExecute(object parameter)
    {
      return true;
    }

    /// <summary>
    /// Fires when the CanExecute status of this command changes.
    /// </summary>
    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }

    /// <summary>
    /// Invokes this command to perform its intended task.
    /// </summary>
    public void Execute(object parameter)
    {
      if (m_ViewModel.IsPaused)
      {
        m_ViewModel.SearchManager.Resume();
        m_ViewModel.PauseResumeButtonText = "Pause";
        m_ViewModel.IsPaused = false;
      }
      else
      {
        m_ViewModel.PauseResumeButtonText = "Resume";
        m_ViewModel.IsPaused = true;
        m_ViewModel.SearchManager.Pause();
      }
    }
  }
}
