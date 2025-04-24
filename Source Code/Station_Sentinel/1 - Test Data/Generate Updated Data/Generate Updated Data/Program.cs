using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/// <summary>
/// Start point that just calls our update generator for the log file.
/// </summary>
internal class Program
{
    static void Main()
    {
        var test = new GenerateNewTimes();
        test.UpdateTestingtimes();
    }
}


/// <summary>
/// Class that contains a single function that just copies and pastes older logs and updates the timestamps for testing purposes.
/// </summary>
class GenerateNewTimes
{
    public void UpdateTestingtimes()
    {

        //List to hold all our dates and start/end dates
        List<DateTime> dates = new List<DateTime>();
        DateTime startDate = DateTime.Now;
        DateTime endDate = DateTime.Now.AddDays(-30);

        //Get our current date and populate 30 days worth of previous dates.
        while (endDate <= startDate.AddDays(1))
        {
            dates.Add(endDate);
            endDate = endDate.AddDays(1);
        }

        //Get our current list of logs from our folder
        string applicationPath = Environment.CurrentDirectory;
        string logPath = applicationPath.Substring(0, applicationPath.IndexOf("Data\\") + 4) + "\\logs\\";
        string[] logFiles = System.IO.Directory.GetFiles(logPath, "", SearchOption.AllDirectories);
        string newDataPath = applicationPath.Substring(0, applicationPath.IndexOf("Data\\") + 4) + "\\CreatedData\\logs";

        //Recreate our directory
        if (System.IO.File.Exists(newDataPath))
        {
            System.IO.File.Delete(newDataPath);
            System.IO.File.Create(newDataPath);
        }

        int logFilesCounter = logFiles.Length - 1; // Generic counter for looping through all the log files.

        dates.Reverse();

        //Loop through and start populating all our file data but modified timestamp data
        foreach (DateTime fileStringDate in dates)
        {
            string[] fileContents = System.IO.File.ReadAllLines(logFiles[logFilesCounter]); //Read file contents
            
            for(int i = 0; i<fileContents.Length -1; i++)
            {

                string newline = "";

                try
                {
                    newline = fileStringDate.ToString("yyyy MM dd ") + fileContents[i].Substring(11, fileContents[i].Length - 11);
                }
                catch
                {
                    newline = fileContents[i];
                }
                
                fileContents[i] = newline;
            }


            string newFilePath = newDataPath + 
                "\\" + fileStringDate.ToString("yyyy") + "\\" +
                fileStringDate.ToString("MM") + "\\" + 
                fileStringDate.ToString("dd") +".txt";
            string newDirectoryPath = newFilePath.Substring(0, newFilePath.Length - 6);
            System.IO.Directory.CreateDirectory(newDirectoryPath);
            System.IO.File.WriteAllLines(newFilePath, fileContents);   //Create the new file.
            logFilesCounter--;  //Decrement for Parity
        }



    }
}

