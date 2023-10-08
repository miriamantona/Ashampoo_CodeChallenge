namespace CodeChallenge.Model
{
  public class DirectoryResult
  {
    public string DirectoryPath { get; set; }
    public int FileCount { get; set; }
    public double TotalSizeMB { get; set; }
    public long TotalSizeBytes { get; set; }
  }
}
