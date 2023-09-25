using CodeChallengeApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

namespace CodeChallenge
{
  public class UIManager
  {
    private MainWindow mainWindow;
    private SearchManager searchManager;
    private DispatcherTimer blinkingTimer;

    public UIManager(MainWindow window)
    {
      mainWindow = window;
      searchManager = new SearchManager();
      searchManager.SearchResultUpdated += SearchManager_SearchResultUpdated;
      searchManager.SearchFinished += SearchManager_SearchFinished;
    }

    public async Task EnableUserSearchAsync()
    {
      var dialog = new FolderBrowserDialog();
      dialog.ShowDialog();
      mainWindow.textBoxFolder.Text = dialog.SelectedPath;
      mainWindow.dataGridResults.Items.Clear();
      mainWindow.buttonSearch.IsEnabled = false;
      mainWindow.buttonPauseResume.IsEnabled = true;
      mainWindow.textNoFiles.Visibility = Visibility.Hidden;
      mainWindow.textErrorMessage.Visibility = Visibility.Hidden;
      InitializeSearchingMessage();

      await searchManager.SearchAsync(dialog.SelectedPath);
    }

    public void PrepareWindowForNewSearch()
    {
      mainWindow.buttonSearch.IsEnabled = true;
      mainWindow.buttonPauseResume.IsEnabled = false;
    }

    public void InitializeSearchingMessage()
    {
      mainWindow.textBoxSearching.Text = "Searching...";
      mainWindow.textBoxSearching.Visibility = Visibility.Visible;
      blinkingTimer = new DispatcherTimer();
      blinkingTimer.Tick += BlinkingTimer_Tick;
      blinkingTimer.Interval = TimeSpan.FromMilliseconds(500);
      blinkingTimer.Start();
    }

    public void PrepareWindowCompletedSearch()
    {
      StopBlinkingTimer();
      mainWindow.textBoxSearching.Text = "Search Completed";
      mainWindow.textBoxSearching.Visibility = Visibility.Visible;

      if (mainWindow.dataGridResults.Items.IsEmpty)
      {
        mainWindow.textNoFiles.Visibility = Visibility.Visible;
      }
    }

    public void Pause()
    {
      searchManager.Pause();
      mainWindow.buttonPauseResume.Content = "Resume";
      StopBlinkingTimer();
    }

    public void Resume()
    {
      searchManager.Resume();
      mainWindow.buttonPauseResume.Content = "Pause";
      InitializeSearchingMessage();
    }

    public void ShowErrorMessage(string message)
    {
      mainWindow.Dispatcher.Invoke(() =>
      {
        mainWindow.textErrorMessage.Text = "Error processing directories: " + message;
        mainWindow.textErrorMessage.Visibility = Visibility.Visible;
        StopBlinkingTimer();
        mainWindow.textBoxSearching.Visibility = Visibility.Hidden;
        PrepareWindowForNewSearch();
      });
    }

    private void SearchManager_SearchResultUpdated(List<DirectoryResult> results)
    {
      if (results.Any())
      {
        mainWindow.Dispatcher.Invoke(() =>
        {
          foreach (var result in results)
          {
            mainWindow.dataGridResults.Items.Add(result);
          }
        });
      }
    }

    private void SearchManager_SearchFinished()
    {
      mainWindow.Dispatcher.Invoke(() =>
      {
        if (!searchManager.HasDirectoriesToProcess())
        {
          PrepareWindowCompletedSearch();
          PrepareWindowForNewSearch();
        }
      });
    }

    private void BlinkingTimer_Tick(object sender, EventArgs e)
    {
      if (mainWindow.textBoxSearching.Visibility == Visibility.Visible)
      {
        mainWindow.textBoxSearching.Visibility = Visibility.Hidden;
      }
      else
      {
        mainWindow.textBoxSearching.Visibility = Visibility.Visible;
      }
    }

    public void StopBlinkingTimer()
    {
      if (blinkingTimer != null)
      {
        blinkingTimer.Stop();
        blinkingTimer.Tick -= BlinkingTimer_Tick;
        blinkingTimer = null;
        mainWindow.textBoxSearching.Visibility = Visibility.Hidden;
      }
    }
  }
}
