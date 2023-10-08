using CodeChallengeApp.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CodeChallengeApp.ViewModel.Commands
{
  public class SearchCommand : ICommand
  {

    // Member variables
    private readonly SearchViewModel m_ViewModel;

    public SearchCommand(SearchViewModel viewModel)
    {
      m_ViewModel = viewModel;
    }

    public bool CanExecute(object parameter)
    {
      return true;
    }

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
          m_ViewModel.IsSearchCompleted = false;
        }
        else
        {
          m_ViewModel.IsSearching = true;
          m_ViewModel.IsSearchCompleted = false;
          m_ViewModel.HasError = false;
          m_ViewModel.DirectoriesResult = new ObservableCollection<Model.DirectoryResult>();


          foreach (var drive in selectedDrives)
          {
            var directoriesQueue = new DirectoriesQueue(drive.Name);
            m_ViewModel.SearchManager.AddQueue(directoriesQueue);
            Task.Run(() => m_ViewModel.SearchManager.SearchAsync(directoriesQueue));
          }
        }
      }
      catch (Exception ex)
      {
        m_ViewModel.HasError = true;
        m_ViewModel.ErrorMessage = ex.Message;
      }
    }
  }
}
