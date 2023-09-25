using CodeChallengeApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeChallenge
{
  public class SearchManager
  {
    private List<string> processedDirectories;
    private CancellationTokenSource cancellationTokenSource;
    private CancellationToken cancellationToken;
    private Queue<string> queueDirectories = new Queue<string>();

    public delegate void SearchResultUpdatedEventHandler(List<DirectoryResult> results);
    public event SearchResultUpdatedEventHandler SearchResultUpdated;

    public delegate void SearchFinishedEventHandler();
    public event SearchFinishedEventHandler SearchFinished;


    public SearchManager()
    {
      processedDirectories = new List<string>();
      cancellationTokenSource = new CancellationTokenSource();
    }

    public async Task SearchAsync(string selectedDrive)
    {
      cancellationTokenSource = new CancellationTokenSource();
      cancellationToken = cancellationTokenSource.Token;

      if (String.IsNullOrEmpty(selectedDrive))
        return;

      if (!queueDirectories.Any())
        processedDirectories = new List<string>();

      List<DirectoryResult> results = new List<DirectoryResult>();

      queueDirectories.Enqueue(selectedDrive);
      List<Task> tasks = new List<Task>();

      await Task.Run(async () =>
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

            await SearchInDirectoryAsync(currentDirectory);

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
        OnSearchFinished();
      });
    }

    public async Task<List<DirectoryResult>> SearchInDirectoryAsync(string directoryPath)
    {
      List<DirectoryResult> results = new List<DirectoryResult>();
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
              DirectoryPath = processedDirectories.Count + 1 + "-" + directoryPath,
              FileCount = GetFileCount(directoryPath),
              TotalSizeMB = GetTotalSizeMB(directoryPath)
            });
            processedDirectories.Add(directoryPath);
            OnSearchResultUpdated(results);
          }
        }
      }

      return results;
    }

    private void OnSearchResultUpdated(List<DirectoryResult> results)
    {
      SearchResultUpdated?.Invoke(results);
    }

    private void OnSearchFinished()
    {
      SearchFinished?.Invoke();
    }

    public bool IsCancelledRequested()
    {
      return cancellationToken.IsCancellationRequested;
    }

    private int GetFileCount(string directory)
    {
      return Directory.GetFiles(directory).Length;
    }

    private double GetTotalSizeMB(string directory)
    {
      double totalSize = 0;

      foreach (string file in Directory.GetFiles(directory))
      {
        totalSize += new FileInfo(file).Length;
      }

      return (totalSize / (1024 * 1024));
    }

    public async void Resume()
    {
      cancellationTokenSource = new CancellationTokenSource();
      if (queueDirectories.Any())
        await SearchAsync(queueDirectories.Dequeue());
      else
        OnSearchFinished();
    }

    public void Pause()
    {
      cancellationTokenSource?.Cancel();
    }

    public bool HasDirectoriesToProcess()
    {
      return queueDirectories.Any();
    }
  }
}
