using CodeChallengeApp.Managers;
using CodeChallengeApp.Model;
using CodeChallengeApp.Utilities;
using CodeChallengeApp.ViewModel.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace CodeChallengeApp.ViewModel
{
  public class SearchViewModel : ViewModelBase
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
          RaisePropertyChangedEvent(nameof(NoResultsInSearch));
        }
      }
    }

    public bool NoResultsInSearch
    {
      get { return _isSearchCompleted && (_directoriesResult == null || _directoriesResult.Count == 0); }
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

    private ObservableCollection<DirectoryResult> _directoriesResult;
    public ObservableCollection<DirectoryResult> DirectoriesResult
    {
      get { return _directoriesResult; }
      set
      {
        if (_directoriesResult != value)
        {
          _directoriesResult = value;
          RaisePropertyChangedEvent(nameof(DirectoriesResult));
        }
      }
    }

    public ICommand Search { get; set; }
    public ICommand PauseResume { get; set; }


    public SearchViewModel()
    {
      this.Initialize();      
    }

    private void Initialize()
    {
      this.Search = new SearchCommand(this);
      this.PauseResume = new PauseResumeCommand(this);

      _searchManager = new SearchManager();
      _searchManager.SearchFinished += HandleSearchFinished;
      _searchManager.SearchResultUpdated += HandleSearchResultUpdated;

      _directoriesResult = new ObservableCollection<DirectoryResult>();

      DriveInfo[] drivesInfo = DriveInfo.GetDrives();
      _drives = new();
      foreach (DriveInfo driveInfo in drivesInfo)
      {
        _drives.Add(new Drive { Name = driveInfo.Name, IsSelected = false });
      }
    }

    private void HandleSearchFinished()
    {
      this.IsSearching = false;
      this.IsSearchCompleted = true;
    }
    private void HandleSearchResultUpdated(List<DirectoryResult> results)
    {
      if (results.Any())
      {
        foreach (var result in results)
        {
          Application.Current.Dispatcher.Invoke(() => DirectoriesResult.Add(result));
        }
      }
    }
  }
}
