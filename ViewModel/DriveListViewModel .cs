using CodeChallengeApp.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeChallenge.ViewModel
{
  public class DriveListViewModel : ViewModelBase
  {
    private ObservableCollection<string> _drives = new ObservableCollection<string>();

    public ObservableCollection<string> Drives
    {
      get { return _drives; }
      set
      {
        _drives = value;
        RaisePropertyChangedEvent(nameof(Drives));
      }
    }

    // Constructor donde obtienes los datos
    public DriveListViewModel()
    {
      DriveInfo[] drivesInfo = DriveInfo.GetDrives();
      foreach (DriveInfo driveInfo in drivesInfo)
      {
        if (driveInfo.IsReady)
        {
          Drives.Add(driveInfo.Name);
        }
      }
    }

    private ObservableCollection<string> _selectedDrives;
    public ObservableCollection<string> SelectedDrives
    {
      get { return _selectedDrives; }
      set
      {
        RaisePropertyChangedEvent(nameof(SelectedDrives));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }

}
