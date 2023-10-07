using CodeChallenge;
using CodeChallenge.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
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

    /// <summary>
    /// Invokes this command to perform its intended task.
    /// </summary>
    public void Execute(object parameter)
    {
      m_ViewModel.IsSearching = true;

      var selectedDrives = m_ViewModel.Drives.Where(d => d.IsSelected).ToList();

      List<Task> taskList = new();

      foreach (var drive in selectedDrives)
      {
        Queue<string> queueDirectories = new Queue<string>();
        queueDirectories.Enqueue(drive.Name);
        m_ViewModel.SearchManager.AddQueue(queueDirectories);

        taskList.Add(Task.Run(() => m_ViewModel.SearchManager.SearchAsync(queueDirectories)));
      }

      Task.Run(() =>
      {
        while (m_ViewModel.SearchManager.HasDirectoriesToProcess)
        {
          m_ViewModel.IsSearching = true;
          Thread.Sleep(100);
        }
        m_ViewModel.IsSearching = false;
      });
      /*
      try
      {
        if (DriveList.SelectedItems.Count == 0)
        {
          textErrorMessage.Text = "Please, select a drive";
          textErrorMessage.Visibility = Visibility.Visible;
        }
        else
        {

        }
      }
      catch (Exception ex)
      {
        uiManager.ShowErrorMessage(ex.Message);
      }*/
    }

    #endregion
  }
}
