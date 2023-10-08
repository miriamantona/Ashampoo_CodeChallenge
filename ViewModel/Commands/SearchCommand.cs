using CodeChallengeApp.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CodeChallengeApp.ViewModel.Commands
{
  public class SearchCommand : ICommand
  {
    #region Fields

    // Member variables
    private readonly MainWindowViewModel m_ViewModel;

    #endregion

    #region Constructor

    public SearchCommand(MainWindowViewModel viewModel)
    {
      m_ViewModel = viewModel;
    }


    #endregion

    #region ICommand Members

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


    public void Execute(object parameter)
    {
      try
      {
        var selectedDrives = m_ViewModel.Drives.Where(d => d.IsSelected).ToList();

        if (selectedDrives.Count == 0)
        {
          m_ViewModel.HasError = true;
          m_ViewModel.ErrorMessage = "Please, select a drive";
        }
        else
        {
          m_ViewModel.IsSearching = true;
          m_ViewModel.IsSearchCompleted = false;
          m_ViewModel.HasError = false;

          List<Task> taskList = new();

          foreach (var drive in selectedDrives)
          {
            var directoriesQueue = new DirectoriesQueue(drive.Name);
            m_ViewModel.SearchManager.AddQueue(directoriesQueue);
            taskList.Add(Task.Run(() => m_ViewModel.SearchManager.SearchAsync(directoriesQueue)));
          }
        }
      }
      catch (Exception ex)
      {
        m_ViewModel.HasError = true;
        m_ViewModel.ErrorMessage = ex.Message;
      }
    }

    #endregion
  }
}
