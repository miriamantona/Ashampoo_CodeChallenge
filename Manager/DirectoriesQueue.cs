using System.Collections.Generic;

namespace CodeChallengeApp.Managers
{
  public class DirectoriesQueue
  {
    private Queue<string> _queue;
    private bool _isCompleted;

    public Queue<string> Queue
    {
      get
      {
        return _queue;
      }
      set
      {
        _queue = value;
      }
    }

    public bool IsCompleted
    {
      get
      {
        return _isCompleted;
      }
      set
      {
        _isCompleted = value;
      }
    }

    public DirectoriesQueue(string directoryName)
    {
      this.Queue = new Queue<string>();
      this.Queue.Enqueue(directoryName);  
      this._isCompleted = false;
    }
  }
}
