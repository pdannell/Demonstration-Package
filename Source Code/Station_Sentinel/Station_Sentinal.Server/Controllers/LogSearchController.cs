using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;

namespace Station_Sentinal.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogSearchController : Controller
    {
        
        [HttpGet(Name = "LogSearch")]
        public async Task<List<string>> LogSearch(string searchText, string sd, string ed ) {

            List<string> searchResult = []; //Return List For Search Results
            string[] fileContents; //Array for holding read file contents temporarily.
            List<DateTime> dateValues = [];
            DateTime startDate, endDate;

            startDate = DateTime.Parse(sd);
            endDate = DateTime.Parse(ed);

            try
            {
                //Get a list of all dates between two dates.
                if (startDate == endDate) //User Selected A Single Date
                {
                    dateValues.Add(startDate);
                }
                else //User Selected a Range
                {
                    for (var dt = startDate; dt <= endDate; dt = dt.AddDays(1))
                    {
                        dateValues.Add(dt);
                    }
                }

                //Loop through all the values for folderpath dates.
                foreach(DateTime folderPath in dateValues)
                {
                    string filePath = "C:\\Logs\\" + folderPath.ToString("yyyy") + "\\" + folderPath.ToString("MM") + "\\" + folderPath.ToString("dd") + ".txt";

                    ////Loop through all files for folderpath dates.
                    //foreach (string file in Directory.GetFiles("C:\\Logs\\" + folderPath.ToString("yyyy") + "\\" + folderPath.ToString("MM") + "\\" + folderPath.ToString("dd")))
                    //{
                    try
                    {
                        //Read the files contents into our placeholder array
                        fileContents = await System.IO.File.ReadAllLinesAsync(filePath);

                        //Check for matching text in our line.
                        for (int line = 0; line <= fileContents.Length - 1; line++)
                        {
                            if (fileContents[line].Contains(searchText)) { searchResult.Add(fileContents[line] + "\n"); }
                        }
                    }
                    catch { }
                }           
            }
            catch (Exception ex) //Print Error Occurances To Log and return error for user display.
            {
                System.IO.File.WriteAllText("C:\\StationController.txt",ex.ToString());
                searchResult.Clear();
                searchResult.Add("ERROR");
                return searchResult;
            }
            return searchResult;
        }
    }
}
