using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Extensions.Configuration;
using System.Runtime.CompilerServices;
using System.Linq;
using System;
using Microsoft.AspNetCore.SignalR.Protocol;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Linq.Expressions;

namespace Station_Sentinal.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ErrorController : Controller
    {
        private readonly string searchText = "ERROR|";                           //Search text for parsing out data.
        private readonly string logPath = "C:\\Logs\\";
        private readonly string errorFolder = "C:\\ErrorLogs\\";
        private readonly List<string> errorList = [];                            //Bargraph Error Data ----, Format is Error, Total
        private readonly List<Tuple<string, string>> graphResult = [];           //Linegraph Error Data ----, Format is Error, Time
        private readonly List<Tuple<string, string, int>> returnList = [];       //List that is physically returned to the HTTPCaller. ----, Format is Error, Time, Counts
        private readonly int timeReadLength = 13;                                //Clip all times to hour only.


        /// <summary>
        /// This request does the initial population of the error log files for use.
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GenErrData")]
        public async Task<List<Tuple<string, string, int>>> GenerateInitialErrorData(string taskType, string startTime, string endTime) {

            try
            {
                switch(taskType)    //Choose a job to run.
                {
                    case ("Initial"): //Call for initial data gathering and file generation.
                        await GenerateInitialData();
                        await GenerateFileContents();
                        await GenerateGraph(startTime, endTime);
                        break;
                    case ("BarGraph"): //Call that returns bargraph data for a specified timeperiod.
                        await GenerateGraph(startTime, endTime);
                        break;
                    case ("LineGraph"): //Call that returns a linegraph for a specified period.
                        await GenerateGraph(startTime, endTime);
                        break;
                }
            }
            catch (Exception ex) //Print Error Occurances To Log and return error for user display.
            {
                System.IO.File.WriteAllText("C:\\StationController.txt", ex.ToString());
            }
            return returnList;
        }

        #region "---Initial Page Call Functions"
        /// <summary>
        /// Function that generates and writes our initial file data for use.
        /// </summary>
        private async Task GenerateInitialData()
        {
            string[] fileContents; //Array for holding read file contents temporarily.

            if (System.IO.Directory.Exists(errorFolder)) { System.IO.Directory.Delete(errorFolder, true); }

            //Create Our Base directory structures.
            System.IO.Directory.CreateDirectory(errorFolder + "ErrorData\\");

            //Loop through all the values for folderpath dates.
            foreach (string folderPath in System.IO.Directory.GetDirectories(logPath, "*", SearchOption.AllDirectories))
            {
                //Loop through all files for folderpath dates.
                foreach (string file in Directory.GetFiles(folderPath))
                {
                    //Read the files contents into our placeholder array
                    fileContents = await System.IO.File.ReadAllLinesAsync(file);

                    //Grab all our data and return the raw results for the bar and line graphs.
                    GatherGraphData(fileContents);
                }
            }
        }

        /// <summary>
        /// Generates all graph dats into our arrays for use.
        /// </summary>
        /// <param name="fileContents"></param>
        /// <returns></returns>
        private void GatherGraphData(string[] fileContents)
        {

            //Check for matching text in our line.
            for (int line = 0; line < fileContents.Length; line++)
            {
                //Check if we have ERROR| on the current line being read.
                if (fileContents[line].Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
                {

                    //Grab the error specific string.
                    string errorName = fileContents[line][
                        fileContents[line].LastIndexOf("ERROR|")..].Replace("ERROR|", "").Trim();

                    //If the error contains a ,(Normally for excess data) strip the rest off.
                    if (errorName.Contains(',') == true) { errorName = errorName[..errorName.IndexOf(',')]; }
                    

                    //If we never found a record, add it.
                    if (errorList.Contains(errorName) == false)
                    {
                        errorList.Add(errorName);
                    }

                    //Add the result to our list with the time.
                    string timeResult = fileContents[line][..(fileContents[line].IndexOf('|') - 6)];
                    graphResult.Add(Tuple.Create(errorName, timeResult)); //Return List For All Error Results
                }
            }
        }

        /// <summary>
        /// Writes our data arrays to files for future reading.
        /// </summary>
        private async Task GenerateFileContents()
        {
            //Populate all Overall Errors values into a CSV for storing.
            foreach(string item in errorList)
            {
                await System.IO.File.AppendAllTextAsync(errorFolder + "ErrorList.txt", 
                    item.Replace("\\", "")
                    .Replace("/", "")
                    .Replace("PASS", "")
                    .Replace("FAIL", "")
                    .Replace(",", "") + "\n");
            }

            //Populate all LineGraph Values into their respective Files.
            foreach (Tuple<string, string> item in graphResult)
            {

                //Massage our string..
                string filePath = errorFolder + "\\ErrorData\\" + 
                    item.Item1
                    .Replace("\\", "")
                    .Replace("/","")
                    .Replace("PASS","")
                    .Replace("FAIL","")
                    .Replace(",","")
                    + ".txt";

                await System.IO.File.AppendAllTextAsync(filePath, item.Item2.ToString()[..timeReadLength] + "\n");
            }
        }
        #endregion

        #region"---Graphing Specific Functions and Returns"
        /// <summary>
        /// This calculates and formulates data into a Fault, Time, Occurance format for returning to the main function call to play with on the GUI.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private async Task GenerateGraph(string startDate, string endDate)
        {

            try
            {
                //Grab our list of errors.
                string[] errorList = await System.IO.File.ReadAllLinesAsync(errorFolder + "ErrorList.txt");

            //Loop through each error file and format our new list as needed.
            foreach (string errorName in errorList)
            {
                //Open the individual error file.
                string[] specificError = await System.IO.File.ReadAllLinesAsync(errorFolder + "ErrorData\\" + errorName + ".txt");

                //Convert our datetime to Ticks
                DateTime startRange = new(
                    int.Parse(startDate.Substring(startDate.Length - 4, 4)),                                  //Year
                    int.Parse(startDate[..startDate.IndexOf('/')]),                                           //Month
                    int.Parse(startDate[..^5][(startDate.IndexOf('/') + 1)..]));                              //Day
                DateTime endRange = new(
                    int.Parse(endDate.Substring(endDate.Length - 4, 4)),                                      //Year
                    int.Parse(endDate[..endDate.IndexOf('/')]),                                               //Month
                    int.Parse(endDate[..^5][(endDate.IndexOf('/') + 1)..]));                                  //Day

                    //Calculate totals on a per hour basis.
                    foreach (string timeOccurance in specificError)
                    {

                        try {   //Try added due to people like me adding invalid dates.

                            //Get the read line in ticks.
                            DateTime timeOccuranceDateTime = new(
                                int.Parse(timeOccurance[..4]),                     //Year
                                int.Parse(timeOccurance.Substring(5, 2)),          //Month
                                int.Parse(timeOccurance.Substring(8, 2)),          //Day
                                int.Parse(timeOccurance.Substring(11, 2)),         //Hour
                                0,
                                0);

                            //Check if the line read falls into the timeframe for the hour.
                            if (timeOccuranceDateTime.Ticks <= endRange.Ticks && timeOccuranceDateTime.Ticks >= startRange.Ticks)
                            {
                                bool matchFound = false;    //Bool for flagging if we need to add to the list or not.

                                //Loop through our list and see if we get any matches.
                                for (int I = 0; I < returnList.Count; I++)
                                {
                                    if (returnList.Count == 0)
                                    {  //Define the new Tupe for population and flag our match having been found.
                                        returnList[I] = new Tuple<string, string, int>(returnList[I].Item1, returnList[I].Item2, returnList[I].Item3 + 1);
                                        matchFound = true;
                                        break;
                                    }

                                    //Check for a matches error name and time
                                    if (returnList[I].Item1.Contains(errorName) && returnList[I].Item2.Contains(timeOccurance))
                                    {
                                        //Define the new Tupe for population and flag our match having been found.
                                        returnList[I] = new Tuple<string, string, int>(returnList[I].Item1, returnList[I].Item2, returnList[I].Item3 + 1);
                                        matchFound = true;
                                    }
                                }

                                //Add one if nothing has been seen.
                                if (!matchFound) { returnList.Add(new Tuple<string, string, int>(errorName, timeOccurance, 1)); }
                            }
                        }
                        catch(Exception) { }    //Someone fed invalid dates(Me) Added to ignore it.
                    }
                }
            }
            catch (Exception)
            {
               
            }
        }
        #endregion
    }
}
