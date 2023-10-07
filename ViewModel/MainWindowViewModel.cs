using CodeChallengeApp.Utilities;
using CodeChallengeApp.ViewModel.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace CodeChallenge.ViewModel
{
  public class MainWindowViewModel : ViewModelBase
  {
    private bool _isSearching;
    public bool IsSearching
    {
      get { return _isSearching; }
      set
      {
        if (_isSearching != value)
        {
          _isSearching = value;
          RaisePropertyChangedEvent(nameof(IsSearching));
        }
      }
    }

    private bool _isPaused;
    public bool IsPaused
    {
      get { return _isPaused; }
      set
      {
        if (_isPaused != value)
        {
          _isPaused = value;
          RaisePropertyChangedEvent(nameof(IsPaused));
        }
      }
    }

    private string _pauseResumeButtonText = "Pause";
    public string PauseResumeButtonText
    {
      get { return _pauseResumeButtonText; }
      set
      {
        if (_pauseResumeButtonText != value)
        {
          _pauseResumeButtonText = value;
          RaisePropertyChangedEvent(nameof(PauseResumeButtonText));
        }
      }
    }

    private SearchManager _searchManager;
    public SearchManager SearchManager
    {
      get
      {
        return _searchManager;
      }
      set
      {
        if (_searchManager != value)
        {
          _searchManager = value;
        }
      }
    }


    public ICommand Search { get; set; }
    public ICommand PauseResume { get; set; }

    public DriveListViewModel DriveListViewModel { get; set; }

    public MainWindowViewModel(SearchManager searchManager)
    {
      this.Initialize();
      SearchManager = searchManager;
    }

    private void Initialize()
    {
      this.Search = new SearchCommand(this);
      this.PauseResume = new PauseResumeCommand(this);
      DriveListViewModel = new DriveListViewModel();
    }
  }
}
