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
    public class GraphWizard : Controller
    {
        private readonly string logPath = "C:\\Logs\\";                             //Generic Log folder Location
        private readonly string graphFolder = "C:\\GraphInformation\\";             //Where to generate our initial data to.
        private readonly List<string> resultList = [];                              //List which contains all graphing data..

        /// <summary>
        /// Multiuse Get Request For Paring Cycle times and Timer Data
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GraphWizard")]
        public async Task<List<string>> RequestCycleInfo(string taskType, string startTime = "", string endTime = "", string dataName = "") {

            try
            {
                switch (taskType)    //Choose a job to run.
                {
                    case ("Initial"): //Call for initial data gathering and file generation.
                        await GenerateInitialGraphData();
                        break;
                    case ("GraphNames"):
                        await GetGraphNames(startTime, endTime);
                        break;
                    case ("GraphData"): //Call that returns a linegraph for a specified period.
                        await GetGraphData(startTime, endTime, dataName);
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
        private async Task GenerateInitialGraphData()
        {
            List<string> rawGraphData = [];     //Array for holding data gathered
            //List<string> graphList = [];    //Array for holding list of all timers pre definition wise.

            //Check if we need to delete files or create a directory due to it not existing.
            if(System.IO.Directory.Exists(graphFolder))
            {
                System.IO.Directory.Delete(graphFolder, true);
                System.IO.Directory.CreateDirectory(graphFolder);
                System.IO.Directory.CreateDirectory(graphFolder + "Data\\");
            }

            //Create Our Base directory structures.
            System.IO.Directory.CreateDirectory(graphFolder);
            System.IO.Directory.CreateDirectory(graphFolder + "Data\\");


            //Loop through all the values for folderpath dates.
            foreach (string folderPath in System.IO.Directory.GetDirectories(logPath, "*", SearchOption.AllDirectories))
            {
                //Loop through all files for folderpath dates.
                foreach (string file in Directory.GetFiles(folderPath))
                {

                    //Loop through our file contents and start populating into our result arrays.
                    foreach (string line in await System.IO.File.ReadAllLinesAsync(file))
                    {
                        //Add every single line of GraphData that we see.
                        if (line.Contains("GRAPHDATA|", StringComparison.CurrentCultureIgnoreCase)) {
                            rawGraphData.Add(line);
                        }
                    }
                }
            }

            //Gather and sort graph data and lists.
            List<String> graphNames= [];
            foreach (string dataLine in rawGraphData)
            {
                if (graphNames.Contains(dataLine.Split("|")[2]
                    .Replace("\\", "")
                    .Replace("/", "")
                    .Replace(",", "")
                    .Replace("GRAPHNAME=", "")) 
                    == false)
                {
                    graphNames.Add(dataLine.Split("|")[2]
                        .Replace("\\","")
                        .Replace("/","")
                        .Replace(",","")
                        .Replace("GRAPHNAME=",""));
                }
            }

            List<String> graphRefinedList = [];
            //Gather a list of all items that match each error.
            foreach (string name in graphNames)
            {

                string graphType = "";
                graphRefinedList.Clear();

                //Grab all matching timers to memory as this processes much quicker.
                foreach (string line in rawGraphData)
                {

                    if (line.Split("|")[2]
                        .Replace("\\", "")
                        .Replace("/", "")
                        .Replace(",", "")  
                        .Replace("GRAPHNAME=","")
                        == name)
                    {

                        graphType = line.Split("|")[3].Replace("GRAPHTYPE=", "");

                        graphRefinedList.Add(line
                        .Replace("\\", "")
                        .Replace("/", "")
                        .Replace(",", "")
                        .Replace("|GRAPHDATA|GRAPHNAME=" + name.Replace("|GRAPHNAME=","") + "|GRAPHTYPE=" + graphType, ""));


                    }
                }

                //Timer Data List should be complete, write the entire file at once.
                await System.IO.File.AppendAllLinesAsync(graphFolder + "Data\\" + name + "_" + graphType + ".txt", graphRefinedList);
            }

            //Breakdown all Timer Names into a single file for reading.
            graphNames.Sort();
            await System.IO.File.AppendAllLinesAsync(graphFolder + "graphNames.txt", graphNames);

        }

        /// <summary>
        /// Function that grabs all our graphing data that we need and returns it to the frontend for rendering.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private async Task GetGraphData(string startDate, string endDate, string dataName)
        {
            //Array for our contents as we process the file.
            string[] fileContents = [];
            
            //Grab our file Dynamically as we gave it a seperate name than the requested and read its contents..
            foreach (string file in System.IO.Directory.GetFiles(graphFolder + "Data"))
            {
                if (file.Contains(dataName))    //Check for a match, if we find it, append it to the file AND graph type to byte 0.
                {
                    fileContents = await System.IO.File.ReadAllLinesAsync(file);
                    resultList.Add(file.Substring(file.LastIndexOf('_') + 1, file.Length - file.LastIndexOf('_') - 5));
                }
            }



            //Convert our datetime to Ticks
            DateTime startRange = new(
                int.Parse(startDate.Substring(startDate.Length - 4, 4)),                                  //Year
                int.Parse(startDate[..startDate.IndexOf('/')]),                                           //Month
                int.Parse(startDate[..^5][(startDate.IndexOf('/') + 1)..]));                              //Day
            DateTime endRange = new(
                int.Parse(endDate.Substring(endDate.Length - 4, 4)),                                      //Year
                int.Parse(endDate[..endDate.IndexOf('/')]),                                               //Month
                int.Parse(endDate[..^5][(endDate.IndexOf('/') + 1)..]));                                //Day

            //Get the contents from our file..
            foreach (string line in fileContents)
            {

                try {   //Try added for invalid time data creations.
                    //Get the read line time in ticks.
                    DateTime timeOccuranceDateTime = new(
                        int.Parse(line[..4]),                     //Year
                        int.Parse(line.Substring(5, 2)),          //Month
                        int.Parse(line.Substring(8, 2)),          //Day
                        int.Parse(line.Substring(11, 2)),         //Hour
                        int.Parse(line.Substring(14, 2)),          //Minutes
                        int.Parse(line.Substring(17, 2)));        //Second

                    //Check if the line read falls into the timeframe for the hour and add it to the return data as needed..
                    if (timeOccuranceDateTime.Ticks <= endRange.Ticks && timeOccuranceDateTime.Ticks >= startRange.Ticks)
                    {
                        resultList.Add(line);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Retrieves and feeds back all graph data between the user chosen dates.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private async Task GetGraphNames(string startDate, string endDate)
        {
            //Prebuild the timeframes we want.
            DateTime startRange = new(
                int.Parse(startDate.Substring(startDate.Length - 4, 4)),                                  //Year
                int.Parse(startDate[..startDate.IndexOf('/')]),                                           //Month
                int.Parse(startDate[..^5][(startDate.IndexOf('/') + 1)..]));                              //Day
            DateTime endRange = new(
                int.Parse(endDate.Substring(endDate.Length - 4, 4)),                                      //Year
                int.Parse(endDate[..endDate.IndexOf('/')]),                                               //Month
                int.Parse(endDate[..^5][(endDate.IndexOf('/') + 1)..]));                                  //Day

            //Loop through our prebuilt files for matches that fall into our timeframe.
            foreach (string file in System.IO.Directory.GetFiles(graphFolder + "Data"))
            {
                foreach(string line in await System.IO.File.ReadAllLinesAsync(file))
                {
                    try { //Try added to ignore invalid possible dates due to tampering.

                        //Check if the line matches what we want, if it does add the timer name without its type to our return.
                        //Get the read line time in ticks.
                        DateTime timeOccuranceDateTime = new(
                            int.Parse(line[..4]),                     //Year
                            int.Parse(line.Substring(5, 2)),          //Month
                            int.Parse(line.Substring(8, 2)),          //Day
                            int.Parse(line.Substring(11, 2)),         //Hour
                            0,
                            0);

                        string fileName = file.Substring(file.LastIndexOf('\\') + 1, file.Length - file.LastIndexOf('\\') - 1); //Last index of Filename
                        string massagedName = fileName.Split('_')[0];       //Cut down version of Filename that doesn't include graph type or extension.

                        if (timeOccuranceDateTime.Ticks <= endRange.Ticks && timeOccuranceDateTime.Ticks >= startRange.Ticks && !resultList.Contains(massagedName))
                        {
                            resultList.Add(massagedName);
                        }
                    }
                    catch { }
                }
            }
        }

    }
}
