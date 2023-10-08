using CodeChallengeApp.ViewModel;
using System.Windows.Controls;

namespace CodeChallengeApp.View
{
  public partial class SearchView : UserControl
  {
    public SearchView()
    {
      InitializeComponent();

      DataContext = new SearchViewModel();
    }
  }
}
