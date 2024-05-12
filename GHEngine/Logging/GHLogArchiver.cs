using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Logging;

public class GHLogArchiver : ILogArchiver
{
    // Constructors.
    public GHLogArchiver() { }


    // Private methods.
    private string GetFormattedDate(DateTime date)
    {
        return $"{date.Year}y{DateNumberToString(date.Month)}m{DateNumberToString(date.Day)}d";
    }

    private string DateNumberToString(int number)
    {
        return number < 10 ? $"0{number}" : number.ToString();
    }

    // Inherited methods.
    public void Archive(string archiveDirectory, string path)
    {
        Directory.CreateDirectory(archiveDirectory);
        DateTime LogDate = File.GetLastWriteTime(path);

        string ArchivePath;
        int LogNumber = 0;
        do
        {
            LogNumber++;
            ArchivePath = Path.Combine(archiveDirectory,
                $"{GetFormattedDate(LogDate)}{(LogNumber > 1 ? $" ({LogNumber})" : null)}.zip");
        }
        while (File.Exists(ArchivePath));

        byte[] LogData = File.ReadAllBytes(path);

        using ZipArchive Archive = new(File.Open(ArchivePath, FileMode.Create), ZipArchiveMode.Create);
        using Stream EntryStream = Archive.CreateEntry($"{GetFormattedDate(LogDate)}.log", CompressionLevel.SmallestSize).Open();
        EntryStream.Write(LogData);
    }
}