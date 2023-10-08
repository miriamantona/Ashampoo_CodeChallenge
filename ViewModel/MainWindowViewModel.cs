using CodeChallengeApp.Managers;
using CodeChallengeApp.Model;
using CodeChallengeApp.Utilities;
using CodeChallengeApp.ViewModel.Commands;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;

namespace CodeChallengeApp.ViewModel
{
  public class MainWindowViewModel : ViewModelBase
  {
    private List<Drive> _drives;
    public List<Drive> Drives
    {
      get { return _drives; }
    }

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

    private bool _isSearchCompleted;
    public bool IsSearchCompleted
    {
      get { return _isSearchCompleted; }
      set
      {
        if (_isSearchCompleted != value)
        {
          _isSearchCompleted = value;
          RaisePropertyChangedEvent(nameof(IsSearchCompleted));
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

    private bool _hasError;

    public bool HasError
    {
      get
      {
        return _hasError;
      }
      set
      {
        if (_hasError != value)
        {
          _hasError = value;
          RaisePropertyChangedEvent(nameof(HasError));
        }
      }
    }

    private string _errorMessage;

    public string ErrorMessage
    {
      get
      {
        return _errorMessage;
      }
      set
      {
        if (_errorMessage != value)
        {
          _errorMessage = value;
          RaisePropertyChangedEvent(nameof(ErrorMessage));
        }
      }
    }


    public ICommand Search { get; set; }
    public ICommand PauseResume { get; set; }


    public MainWindowViewModel(SearchManager searchManager)
    {
      this.Initialize();
      SearchManager = searchManager;
      SearchManager.SearchFinished += HandleSearchFinished;

      DriveInfo[] drivesInfo = DriveInfo.GetDrives();
      _drives = new();
      foreach (DriveInfo driveInfo in drivesInfo)
      {
        if (driveInfo.IsReady)
        {
          _drives.Add(new Drive { Name = driveInfo.Name, IsSelected = false });
        }
      }
    }

    private void Initialize()
    {
      this.Search = new SearchCommand(this);
      this.PauseResume = new PauseResumeCommand(this);
    }

    private void HandleSearchFinished()
    {
      this.IsSearching = false;
      this.IsSearchCompleted = true;
    }
  }
}
