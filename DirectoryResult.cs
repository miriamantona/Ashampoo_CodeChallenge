using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeChallenge
{
  public class DirectoryResult
  {
    public string DirectoryPath { get; set; }
    public int FileCount { get; set; }
    public double TotalSizeMB { get; set; }
    public long TotalSizeBytes { get; set; }
  }
}
