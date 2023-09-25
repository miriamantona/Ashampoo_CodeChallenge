using CodeChallenge;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace CodeChallengeApp
{
  public partial class MainWindow : System.Windows.Window
  {
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
      try
      {
        await uiManager.EnableUserSearchAsync();
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
