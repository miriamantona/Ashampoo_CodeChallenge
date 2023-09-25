using CodeChallenge;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace CodeChallengeApp
{
  public partial class MainWindow : System.Windows.Window
  {
    private List<DirectoryResult> results = new List<DirectoryResult>();
    private bool isPaused = false;
    SearchManager searchManager;
    private UIManager uiManager;

    public MainWindow()
    {
      InitializeComponent();
      textBoxSearching.Visibility = Visibility.Hidden;
      textNoFiles.Visibility = Visibility.Hidden;
      searchManager = new SearchManager();
      uiManager = new UIManager(this, searchManager);
    }

    // Event handlers
    private async void ButtonSelectFolder_ClickAsync(object sender, RoutedEventArgs e)
    {
      var dialog = new FolderBrowserDialog();
      dialog.ShowDialog();
      textBoxFolder.Text = dialog.SelectedPath;
      dataGridResults.Items.Clear();
      buttonSearch.IsEnabled = false;
      buttonPauseResume.IsEnabled = true;
      textNoFiles.Visibility = Visibility.Hidden;
      uiManager.InitializeSearchingMessage();

      try
      {
        await searchManager.SearchAsync(dialog.SelectedPath);
      }
      catch (Exception ex)
      {
        Dispatcher.Invoke(() =>
        {
          textErrorMessage.Text = "Error processing directories: " + ex.Message;
        });
      }
    }

    private async void ButtonPauseResume_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (isPaused)
        {
          if (searchManager.HasDirectoriesToProcess())
          {
            searchManager.Resume();
            buttonPauseResume.Content = "Pause";
            isPaused = false;
            uiManager.InitializeSearchingMessage();
          }
          else
            uiManager.PrepareWindowCompletedSearch();
        }
        else
        {
          searchManager.Pause();
          buttonPauseResume.Content = "Resume";
          uiManager.StopBlinkingTimer();
          isPaused = true;
          await Task.Delay(100);
        }
      }
      catch (Exception ex)
      {
        Dispatcher.Invoke(() =>
        {
          textErrorMessage.Text = "Error processing directories: " + ex.Message;
        });
      }
    }
  }
}
