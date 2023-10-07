using CodeChallenge;
using CodeChallenge.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

      //Cambiar a SELECTED!!

      DriveInfo[] drivesInfo = DriveInfo.GetDrives();
      foreach (DriveInfo driveInfo in drivesInfo)
      {
        if (driveInfo.IsReady)
        {
          List<Task> tasks = new List<Task>();

          tasks.Add(Task.Run(async () =>
          {
            Queue<string> queueDirectories = new Queue<string>();
            queueDirectories.Enqueue(driveInfo.Name);
            m_ViewModel.SearchManager.AddQueue(queueDirectories);
            m_ViewModel.SearchManager.SearchAsync(queueDirectories);
          }));
        }
      }

        //await Task.WhenAll(tasks);

        //uiManager.PrepareWindowCompletedSearch();


        /*foreach (string selectedDrive in selectedDrives)
        {
          // Hacer algo con selectedDrive...
        }*/
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
        //var selectedItem = m_ViewModel.SelectedItem;
        //m_ViewModel.GroceryList.Remove(selectedItem);
      }

      #endregion
    }
  }
