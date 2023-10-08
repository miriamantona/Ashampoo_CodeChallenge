using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeChallengeApp.Model;

namespace CodeChallengeApp.Managers
{
  public class SearchManager
  {
    public delegate void SearchResultUpdatedEventHandler(List<DirectoryResult> results);
    public event SearchResultUpdatedEventHandler SearchResultUpdated;

    public delegate void SearchFinishedEventHandler();
    public event SearchFinishedEventHandler SearchFinished;

    private CancellationTokenSource cancellationTokenSource;

    private List<DirectoriesQueue> queues = new List<DirectoriesQueue>();
    private List<string> processedDirectories;

    public void AddDirectoryQueue(DirectoriesQueue queue)
    {
      queues.Add(queue);
    }

    public SearchManager()
    {
      processedDirectories = new List<string>();
      cancellationTokenSource = new CancellationTokenSource();
    }

    public async Task SearchAsync(DirectoriesQueue directoriesQueue)
    {
      if (directoriesQueue == null)
        return;

      if (!directoriesQueue.Queue.Any())
        processedDirectories = new List<string>();

      List<DirectoryResult> results = new List<DirectoryResult>();

      _ = Task.Run(() =>
      {
        while (directoriesQueue.Queue.Count > 0)
        {
          string currentDirectory = directoriesQueue.Queue.Dequeue();

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
              directoriesQueue.Queue.Enqueue(subDirectory);
            }
          }
          catch (Exception ex)
          {
            //Log the error or whatever applies.
            Console.WriteLine($"Error processing directory {currentDirectory}: {ex.Message}");
          }
        }
        if (!cancellationTokenSource.IsCancellationRequested)
        {
          directoriesQueue.IsCompleted = true;
          if (!queues.Any(q => q.IsCompleted == false))
          {
            OnSearchFinished();
          }
        }
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
        Task.Run(() =>
        {
          SearchAsync(queue);
        });
      }
    }

    public void Pause()
    {
      cancellationTokenSource?.Cancel();
    }
  }
}
