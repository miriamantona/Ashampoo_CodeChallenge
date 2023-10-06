using CodeChallenge;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace CodeChallengeApp
{
  public partial class MainWindow : System.Windows.Window
  {
    private bool isPaused = false;
    private UIManager uiManager;

    public MainWindow()
    {
      InitializeComponent();
      textBoxSearching.Visibility = Visibility.Hidden;
      textNoFiles.Visibility = Visibility.Hidden;

      List<string> drives = new List<string>();

      DriveInfo[] driveInfos = DriveInfo.GetDrives();
      foreach (DriveInfo driveInfo in driveInfos)
      {
        drives.Add(driveInfo.Name);
      }

      DriveList.ItemsSource = drives;

      uiManager = new UIManager(this);
    }

    // Event handlers
    private async void ButtonSearch_ClickAsync(object sender, RoutedEventArgs e)
    {
      try
      {
        if (DriveList.SelectedItems.Count == 0)
        {
          textErrorMessage.Text = "Please, select a drive";
          textErrorMessage.Visibility = Visibility.Visible;
        }
        else
        {
          uiManager.PrepareWindowForActiveSearch();

          foreach (string selectedDrive in DriveList.SelectedItems)
          {
            Task.Run(async () =>
            {
              DriveInfo drive = new DriveInfo(selectedDrive);

              if (drive.IsReady)
              {
                uiManager.SearchAsync(drive);
              }
            });
          }
        }
      }
      catch (Exception ex)
      {
        uiManager.ShowErrorMessage(ex.Message);
      }
    }

    private async void ButtonPauseResume_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (isPaused)
        {
          uiManager.Resume();
          isPaused = false;
        }
        else
        {
          isPaused = true;
          uiManager.Pause();
          await Task.Delay(100);
        }
      }
      catch (Exception ex)
      {
        uiManager.ShowErrorMessage(ex.Message);
      }
    }
  }
}
