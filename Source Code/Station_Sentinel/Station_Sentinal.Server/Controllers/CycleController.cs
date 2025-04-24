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
    public class CycleController : Controller
    {
        private readonly string logPath = "C:\\Logs\\";                             //Generic Log folder Location
        private readonly string cycleFolder = "C:\\CycleInformation\\";             //Where to generate our initial data to.
        private readonly List<string> resultList = [];                              //List which contains the BCMP through times.

        /// <summary>
        /// Multiuse Get Request For Paring Cycle times and Timer Data
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetCycleInfo")]
        public async Task<List<string>> RequestCycleInfo(string taskType, string startTime = "", string endTime = "", string timerName = "") {

            try
            {
                switch (taskType)    //Choose a job to run.
                {
                    case ("Initial"): //Call for initial data gathering and file generation.
                        await GenerateInitialData();
                        break;
                    case ("CycleTimes"): //Call that returns bargraph data for a specified timeperiod.
                        await GetCycleData(startTime, endTime);
                        break;
                    case ("TimerNames"):
                        await GetTimerNames();
                        break;
                    case ("TimerData"): //Call that returns a linegraph for a specified period.
                        await GetTimerData(startTime, endTime, timerName);
                        break;
                }
            }
            catch (Exception ex) //Print Error Occurances To Log and return error for user display.
            {
                System.IO.File.WriteAllText("C:\\StationController.txt", ex.ToString());
            }
            return (resultList);
        }

        /// <summary>
        /// Function that generates and writes our initial file data for use.
        /// </summary>
        private async Task GenerateInitialData()
        {
            List<string> bcmpList = [];     //Array for holding our simplified BCMPs
            List<string> timerList = [];    //Array for holdinga list of all timers pre definition wise.

            //Check if we need to delete files or create a directory due to it not existing.
            if(System.IO.Directory.Exists(cycleFolder))
            {
                System.IO.Directory.Delete(cycleFolder, true);
                System.IO.Directory.CreateDirectory(cycleFolder);
                System.IO.Directory.CreateDirectory(cycleFolder + "Data\\");
            }

            //Create Our Base directory structures.
            System.IO.Directory.CreateDirectory(cycleFolder);
            System.IO.Directory.CreateDirectory(cycleFolder + "Data\\");


            //Loop through all the values for folderpath dates.
            foreach (string folderPath in System.IO.Directory.GetDirectories(logPath, "*", SearchOption.AllDirectories))
            {
                //Loop through all files for folderpath dates.
                foreach (string file in Directory.GetFiles(folderPath))
                {

                    //Loop through our file contents and start populating into our result arrays.
                    foreach (string line in await System.IO.File.ReadAllLinesAsync(file))
                    {
                        //Parse out to our easy to use arrays for each Timer and BCMP P/F
                        if (line.Contains("TIMER|", StringComparison.CurrentCultureIgnoreCase)) {

                            try //Try to read information, and ignore invalid data.
                            {
                                if (DateTime.Now.Ticks - DateTime.Parse(line[..10]).Ticks < TimeSpan.TicksPerDay * 7)
                                {
                                    timerList.Add(line);
                                }
                            }
                            catch { }
                        }
                        else if (line.Contains("BCMP", StringComparison.CurrentCultureIgnoreCase) && line.Contains("STATUS", StringComparison.CurrentCultureIgnoreCase) && line.Contains("PASS", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (DateTime.Now.Ticks - DateTime.Parse(line[..10]).Ticks < TimeSpan.TicksPerDay * 7)
                            {
                                bcmpList.Add(line[..19] + "|" + "P");
                            }
                        }
                        else if (line.Contains("BCMP", StringComparison.CurrentCultureIgnoreCase) && line.Contains("STATUS", StringComparison.CurrentCultureIgnoreCase) && line.Contains("FAIL", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (DateTime.Now.Ticks - DateTime.Parse(line[..10]).Ticks < TimeSpan.TicksPerDay * 7)
                            {
                                bcmpList.Add(line[..19] + "|" + "F");
                            }
                        }
                    }
                }
            }

            //Gather a list of each Timer.
            List<String> timerNames= [];
            List<String> TimerTimes = [];
            foreach (string error in timerList)
            {
                if (timerNames.Contains(error.Split("|")[2]) == false)
                {
                    timerNames.Add(error.Split("|")[2]
                        .Replace("\\","").Replace("/","").Replace(",",""));
                }
            }

            //Gather a list of all items that match each error.
            foreach (string timer in timerNames)
            {
                //Grab all matching timers to memory as this processes much quicker.
                foreach (string line in timerList)
                {
                    if (line.Split("|")[2]
                        .Replace("\\", "").Replace("/", "").Replace(",", "") 
                        == timer)
                    {
                        TimerTimes.Add(line
                        .Replace("\\", "").Replace("/", "").Replace(",", "").Replace("TIMER|","").Replace(timer + "|",""));
                    }
                }

                //Error check should be complete, write the entire file at once.
                await System.IO.File.AppendAllLinesAsync(cycleFolder + "Data\\" + timer + ".txt", TimerTimes);
            }

            //Breakdown all BCMPS into a single file for reading.
            await System.IO.File.AppendAllLinesAsync(cycleFolder + "BCMPInfo.txt", bcmpList);

            //Breakdown all Timernames into a single file for reading.
            timerNames.Sort();
            await System.IO.File.AppendAllLinesAsync(cycleFolder + "TimerNames.txt", timerNames);
        }


        /// <summary>
        /// Function that grabs our overall cycle timer information for graphing.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private async Task GetCycleData(string startDate, string endDate)
        {
            string[] fileContents = await System.IO.File.ReadAllLinesAsync(cycleFolder + "BCMPInfo.txt");   //Read the pre generated file contents that we have.

            //Convert our datetime to Ticks
            DateTime startRange = new(
                int.Parse(startDate.Substring(startDate.Length - 4, 4)),                                  //Year
                int.Parse(startDate[..startDate.IndexOf('/')]),                                           //Month
                int.Parse(startDate[..^5][(startDate.IndexOf('/') + 1)..]));                              //Day
            DateTime endRange = new(
                int.Parse(endDate.Substring(endDate.Length - 4, 4)),                                      //Year
                int.Parse(endDate[..endDate.IndexOf('/')]),                                               //Month
                int.Parse(endDate[..^5][(endDate.IndexOf('/') + 1)..]));                                  //Day
            DateTime previousLineTime = new();                          //Holder value for calculating the time differences.

            //Get the contents from our file..
            foreach (string line in fileContents)
            {

                //Get the read line time in ticks.
                DateTime timeOccuranceDateTime = new(
                    int.Parse(line[..4]),                     //Year
                    int.Parse(line.Substring(5, 2)),          //Month
                    int.Parse(line.Substring(8, 2)),          //Day
                    int.Parse(line.Substring(11, 2)),         //Hour
                    int.Parse(line.Substring(14, 2)),          //Minutes
                    int.Parse(line.Substring(17, 2)));        //Second

                bool invalidRecord = false;


                //Check if the line read falls into the timeframe for the hour.
                if (timeOccuranceDateTime.Ticks >= endRange.Ticks && timeOccuranceDateTime.Ticks <= startRange.Ticks)
                {

                    //Check that this isn't the very first line to add.
                    if(previousLineTime != DateTime.MinValue)
                    {

                        //Calculate our time difference to give us the raw BCMP To Timer Value and append the pass/fail that we can see.
                        int timeDifference = (int)(timeOccuranceDateTime - previousLineTime).TotalSeconds;
                        if (timeDifference < 3) { invalidRecord = true; }
                        if (invalidRecord == false) { resultList.Add(string.Concat(timeDifference.ToString(), line.AsSpan(line.Length - 2, 2))); }
                    }
                    if (invalidRecord == false) { previousLineTime = timeOccuranceDateTime; } //Store the last line reads value for comparing against our nextread.
                }
            }
        }

        /// <summary>
        /// Retrieves and feeds back our timer names for populating our listing for the user..
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private async Task GetTimerNames()
        {
            string[] fileContents = await System.IO.File.ReadAllLinesAsync(cycleFolder + "TimerNames.txt");   //Read the pre generated file contents that we have.
        
            //Loop through and populate all the contents to a readable array.
            foreach(string line in fileContents)
            {
                resultList.Add(line);
            }
        }

        /// <summary>
        /// Function that returns a timers data for the range the user requests.
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="timerName"></param>
        /// <returns></returns>
        private async Task GetTimerData(string startDate, string endDate, string timerName)
        {
            string[] fileContents = await System.IO.File.ReadAllLinesAsync(cycleFolder + "Data\\" + timerName + ".txt");   //Read the pre generated file contents that we have.

            //Convert our datetime to Ticks
            DateTime startRange = new(
                int.Parse(startDate.Substring(startDate.Length - 4, 4)),                                  //Year
                int.Parse(startDate[..startDate.IndexOf('/')]),                                           //Month
                int.Parse(startDate[..^5][(startDate.IndexOf('/') + 1)..]));                              //Day
            DateTime endRange = new(
                int.Parse(endDate.Substring(endDate.Length - 4, 4)),                                      //Year
                int.Parse(endDate[..endDate.IndexOf('/')]),                                               //Month
                int.Parse(endDate[..^5][(endDate.IndexOf('/') + 1)..]));                                //Day

            //Loop through each line of our file contents.
            foreach (string line in fileContents)
            {
                //Get the read line time in ticks.
                DateTime timeOccuranceDateTime = new(
                    int.Parse(line[..4]),                     //Year
                    int.Parse(line.Substring(5, 2)),          //Month
                    int.Parse(line.Substring(8, 2)),          //Day
                    int.Parse(line.Substring(11, 2)),         //Hour
                    0,
                    0);

                //Check if the line read falls into the timeframe for the hour.
                if (timeOccuranceDateTime.Ticks >= endRange.Ticks && timeOccuranceDateTime.Ticks <= startRange.Ticks)
                {
                    resultList.Add(line);
                }
            }
        }
    }
}
