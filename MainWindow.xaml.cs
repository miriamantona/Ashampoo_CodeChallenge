using CodeChallenge;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace CodeChallengeApp
{
  public partial class MainWindow : System.Windows.Window
  {
    private List<string> processedDirectories;
    private List<DirectoryResult> results = new List<DirectoryResult>();
    private int totalDirectoriesToProcess;
    private int processedDirectoriesCount;
    private string selectedDrive;
    private CancellationTokenSource cancellationTokenSource;
    private bool isPaused = false;


    public MainWindow()
    {
      InitializeComponent();
      progressBar.Visibility = Visibility.Hidden;
      textNoFiles.Visibility = Visibility.Hidden;
      cancellationTokenSource = new CancellationTokenSource();
    }

    public class DriveInformation
    {
      public string DriveLetter { get; set; }
      public string VolumeLabel { get; set; }
    }

    // Event handlers
    private async void ButtonSelectFolder_ClickAsync(object sender, RoutedEventArgs e)
    {
      var dialog = new FolderBrowserDialog();
      dialog.ShowDialog();
      selectedDrive = textBoxFolder.Text = dialog.SelectedPath;
      dataGridResults.Items.Clear();
      progressBar.Visibility = Visibility.Visible;
      buttonPauseResume.IsEnabled = true;
      await StartSearchAsync(dialog.SelectedPath);
    }

    private async void ButtonPauseResume_Click(object sender, RoutedEventArgs e)
    {
      if (isPaused) //Click Resume
      {
        UpdateProgressBar(); // Actualizar el progreso
        buttonPauseResume.Content = "Pause";
        isPaused = false;
        cancellationTokenSource = new CancellationTokenSource();
        await StartSearchAsync(selectedDrive);
      }
      else // Click pause
      {
        buttonPauseResume.Content = "Resume";
        cancellationTokenSource?.Cancel();
        isPaused = true;
        await Task.Delay(100); // Permitir que se procese la cancelación        
      }
    }

    public async Task StartSearchAsync(string selectedDrive)
    {
      cancellationTokenSource = new CancellationTokenSource();
      CancellationToken cancellationToken = cancellationTokenSource.Token;
      totalDirectoriesToProcess = GetDirectoriesToProcess(selectedDrive);
      textNoFiles.Visibility = Visibility.Hidden;
      progressBar.Value = 0;

      List<DirectoryResult> results = new List<DirectoryResult>();

      await Task.Run(() =>
      {
        foreach (string subDir in Directory.EnumerateDirectories(selectedDrive))
        {
          results = SearchInDirectoryAsync(subDir, cancellationToken).Result;

          if (cancellationToken.IsCancellationRequested)
          {
            break;
          }
          else
          {
            UpdateView(results);
          }
        }

        results = SearchInDirectoryAsync(selectedDrive, cancellationToken).Result;
        if (!cancellationToken.IsCancellationRequested)
        {
          UpdateView(results);
        }
      });

      if (dataGridResults.Items.IsEmpty)
      {
        textNoFiles.Visibility = Visibility.Visible;
        progressBar.Value = 100;
      }
    }

    public async Task<List<DirectoryResult>> SearchInDirectoryAsync(string directoryPath, CancellationToken cancellationToken)
    {
      List<DirectoryResult> results = new List<DirectoryResult>();
      processedDirectories = new List<string>();

      try
      {
        foreach (FileInfo file in new DirectoryInfo(directoryPath).GetFiles("*.*", SearchOption.TopDirectoryOnly))
        {
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
            }
          }
        }

        List<Task<List<DirectoryResult>>> subDirTasks = new List<Task<List<DirectoryResult>>>();

        foreach (string subDir in Directory.EnumerateDirectories(directoryPath))
        {
          subDirTasks.Add(SearchInDirectoryAsync(subDir, cancellationToken));
        }

        Task.WaitAll(subDirTasks.ToArray());

        foreach (var task in subDirTasks)
        {
          results.AddRange(task.Result);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("Error: " + ex.Message);
      }

      return results;
    }

    private void UpdateView(List<DirectoryResult> results)
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
      processedDirectoriesCount++;
      UpdateProgressBar();
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

    private int GetDirectoriesToProcess(string selectedDrive)
    {
      if (!Directory.EnumerateDirectories(selectedDrive).Any())
      {
        return Directory.EnumerateFiles(selectedDrive).Count();
      }
      return Directory.EnumerateDirectories(selectedDrive).Count();
    }

    private void UpdateProgressBar()
    {
      Dispatcher.Invoke(() =>
      {
        double progressPercentage = ((double)processedDirectoriesCount / totalDirectoriesToProcess) * 100;
        progressBar.Value = progressPercentage;
      });
    }


  }
}
