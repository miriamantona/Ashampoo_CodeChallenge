using CodeChallenge;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

namespace CodeChallengeApp
{
  public partial class MainWindow : System.Windows.Window
  {
    private List<string> processedDirectories;
    private List<DirectoryResult> results = new List<DirectoryResult>();
    private CancellationTokenSource cancellationTokenSource;
    private bool isPaused = false;
    private Queue<string> queueDirectories = new Queue<string>();
    private DispatcherTimer blinkingTimer;
    private CancellationToken cancellationToken;

    public MainWindow()
    {
      InitializeComponent();
      textBoxSearching.Visibility = Visibility.Hidden;
      textNoFiles.Visibility = Visibility.Hidden;
      cancellationTokenSource = new CancellationTokenSource();     
    }

    // Event handlers
    private async void ButtonSelectFolder_ClickAsync(object sender, RoutedEventArgs e)
    {
      var dialog = new FolderBrowserDialog();
      dialog.ShowDialog();
      textBoxFolder.Text = dialog.SelectedPath;
      dataGridResults.Items.Clear();
      buttonPauseResume.IsEnabled = true;
      buttonSearch.IsEnabled = false;
      InitializeSearchingMessage();

      await StartSearchAsync(dialog.SelectedPath);

      if (!cancellationToken.IsCancellationRequested)
      {
        buttonSearch.IsEnabled = true;
        ShowCompletedSearchMessage();       
      }
    }

    private async void ButtonPauseResume_Click(object sender, RoutedEventArgs e)
    {
      if (isPaused) //Click Resume
      {
        buttonPauseResume.Content = "Pause";
        isPaused = false;
        InitializeSearchingMessage();
        cancellationTokenSource = new CancellationTokenSource();
        await StartSearchAsync(queueDirectories.Dequeue());
      }
      else // Click pause
      {
        buttonPauseResume.Content = "Resume";
        StopBlinkingTimer();
        cancellationTokenSource?.Cancel();
        isPaused = true;
        await Task.Delay(100); // Permitir que se procese la cancelación        
      }
    }

    public async Task StartSearchAsync(string selectedDrive)
    {
      if (String.IsNullOrEmpty(selectedDrive))
        return;

      processedDirectories = new List<string>();
      cancellationTokenSource = new CancellationTokenSource();
      cancellationToken = cancellationTokenSource.Token;
      textNoFiles.Visibility = Visibility.Hidden;

      List<DirectoryResult> results = new List<DirectoryResult>();

      queueDirectories.Enqueue(selectedDrive);
      List<Task> tasks = new List<Task>();

      await Task.Run(async() =>
      {
        while (queueDirectories.Count > 0)
        {
          string currentDirectory = queueDirectories.Dequeue();         

          try
          {
            if (cancellationToken.IsCancellationRequested)
            {
              break;
            }

            await SearchInDirectoryAsync(currentDirectory, cancellationToken);

            string[] subDirectories = Directory.GetDirectories(currentDirectory);
            foreach (string subDirectory in subDirectories)
            {
              queueDirectories.Enqueue(subDirectory);
            }
          }
          catch (Exception ex)
          {
            Console.WriteLine($"Error al procesar el directorio {currentDirectory}: {ex.Message}");
          }
        }
      });
      if (dataGridResults.Items.IsEmpty)
      {
        textNoFiles.Visibility = Visibility.Visible;
      }
    }

    public async Task<List<DirectoryResult>> SearchInDirectoryAsync(string directoryPath, CancellationToken cancellationToken)
    {
      List<DirectoryResult> results = new List<DirectoryResult>();
      try
      {
        cancellationToken.ThrowIfCancellationRequested();

        foreach (FileInfo file in new DirectoryInfo(directoryPath).GetFiles("*.*", SearchOption.TopDirectoryOnly))
        {
          cancellationToken.ThrowIfCancellationRequested();

          if (!processedDirectories.Contains(directoryPath))
          {
            if (file.Length > 10 * 1024 * 1024) // Verifica si el archivo es mayor a 10 MB
            {
              results.Add(new DirectoryResult
              {
                DirectoryPath = directoryPath,
                FileCount = GetFileCount(directoryPath),
                TotalSizeMB = GetTotalSizeMB(directoryPath),
                TotalSizeBytes = GetTotalSizeBytes(directoryPath)
              });
              processedDirectories.Add(directoryPath);
              UpdateGrid(results);
            }
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("Error: " + ex.Message);
      }

      return results;
    }

    private void UpdateGrid(List<DirectoryResult> results)
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

    private int GetFileCount(string directory)
    {
      return Directory.GetFiles(directory).Length;
    }

    private double GetTotalSizeMB(string directory)
    {
      double totalSizeBytes = 0;

      foreach (string file in Directory.GetFiles(directory))
      {
        totalSizeBytes += new FileInfo(file).Length;
      }

      return (totalSizeBytes / (1024 * 1024));
    }

    private long GetTotalSizeBytes(string directory)
    {
      long totalSizeBytes = 0;

      foreach (string file in Directory.GetFiles(directory))
      {
        totalSizeBytes += new FileInfo(file).Length;
      }

      return totalSizeBytes;
    }

    private void InitializeSearchingMessage()
    {
      textBoxSearching.Text = "Searching...";
      textBoxSearching.Visibility = Visibility.Visible;
      blinkingTimer = new DispatcherTimer();
      blinkingTimer.Tick += BlinkingTimer_Tick;
      blinkingTimer.Interval = TimeSpan.FromMilliseconds(500); // Cambia la velocidad de parpadeo según sea necesario
      blinkingTimer.Start();
    }

    private void BlinkingTimer_Tick(object sender, EventArgs e)
    {
      if (textBoxSearching.Visibility == Visibility.Visible)
      {
        textBoxSearching.Visibility = Visibility.Hidden;
      }
      else
      {
        textBoxSearching.Visibility = Visibility.Visible;
      }
    }

    private void StopBlinkingTimer()
    {
      if (blinkingTimer != null)
      {
        blinkingTimer.Stop();
        blinkingTimer.Tick -= BlinkingTimer_Tick;
        blinkingTimer = null;
        textBoxSearching.Visibility = Visibility.Hidden;
      }
    }

    private void ShowCompletedSearchMessage()
    {
      StopBlinkingTimer();
      textBoxSearching.Text = "Search Completed";
      textBoxSearching.Visibility = Visibility.Visible;
    }
  }
}
