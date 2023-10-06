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

    public delegate void SearchResultUpdatedEventHandler(List<DirectoryResult> results);
    public event SearchResultUpdatedEventHandler SearchResultUpdated;

    public delegate void SearchFinishedEventHandler();
    public event SearchFinishedEventHandler SearchFinished;

    public CancellationTokenSource cancellationTokenSource;

    private List<Queue<string>> queues = new List<Queue<string>>();

    public void AddQueue(Queue<string> queue)
    {
      this.queues.Add(queue);
    }


    public SearchManager()
    {
      processedDirectories = new List<string>();
      cancellationTokenSource = new CancellationTokenSource();
    }

    public async Task SearchAsync(Queue<string> queueDirectories)
    {
      if (queueDirectories == null)
        return;

      if (!queueDirectories.Any())
        processedDirectories = new List<string>();

      List<DirectoryResult> results = new List<DirectoryResult>();

      _ = Task.Run(async() =>
      {
        while (queueDirectories.Count > 0)
        {
          string currentDirectory = queueDirectories.Dequeue();

          try
          {
            if (cancellationTokenSource.IsCancellationRequested)
            {
              break;
            }

            SearchInDirectoryAsync(currentDirectory);

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
        //OnSearchFinished();
      });
    }

    public async Task<List<DirectoryResult>> SearchInDirectoryAsync(string directoryPath)
    {
      List<DirectoryResult> results = new List<DirectoryResult>();

      foreach (FileInfo file in new DirectoryInfo(directoryPath).GetFiles("*.*", SearchOption.TopDirectoryOnly))
      {
        if (cancellationTokenSource.IsCancellationRequested)
        {
          break;
        }

        if (!processedDirectories.Contains(directoryPath))
        {
          if (file.Length > 10 * 1024 * 1024)
          {
            results.Add(new DirectoryResult
            {
              DirectoryPath = directoryPath,
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

      return Math.Round(totalSize / (1024 * 1024), 2);
    }

    public async void Resume()
    {
      cancellationTokenSource = new CancellationTokenSource();
      foreach (var queue in queues)
      {
        Task.Run(async () =>
        {
          SearchAsync(queue);
        });
      }

      /*if (queueDirectories.Any())
        await SearchAsync(queueDirectories.Dequeue());
      else
        OnSearchFinished();*/
    }

    public void Pause()
    {
      cancellationTokenSource?.Cancel();
    }
    public bool HasDirectoriesToProcess()
    {
      return queues.Any();
    }
  }
}
