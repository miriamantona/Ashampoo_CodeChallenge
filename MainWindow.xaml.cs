using CodeChallenge;
using CodeChallenge.Model;
using CodeChallenge.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace CodeChallengeApp
{
  public partial class MainWindow : System.Windows.Window
  {
    SearchManager searchManager;

    public MainWindow()
    {
      InitializeComponent();

      searchManager = new SearchManager();
      DataContext = new MainWindowViewModel(searchManager);

      searchManager.SearchResultUpdated += HandleSearchResultUpdated;
    }

    private void HandleSearchResultUpdated(List<DirectoryResult> results)
    {
      if (results.Any())
      {
        Dispatcher.Invoke(() =>
        {
          foreach (var result in results)
          {
            dataGridResults.Items.Add(result);
          }
        });
      }
    }    
  }
}
