using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Extensions.Configuration;
using System.Reflection.Metadata;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Station_Sentinal.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StationInformationController : Controller
    {

        //Setup local library import for getting the time the user was last active on the mouse.
        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        //Create our struct for data retrieval.
        private struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        //Create our function to get the idle time.
        private uint GetIdleTime()
        {
            LASTINPUTINFO lastInputInfo = new();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
            GetLastInputInfo(ref lastInputInfo);
            uint idleTime = ((uint)Environment.TickCount - lastInputInfo.dwTime);
            return idleTime;
        }

        [HttpGet(Name = "StationInformation")]
        public async Task<List<string>> GetStationInfo() {

            //Initialize our list for feedback to to our HTTP call.
            List<string> returnString = [];

            //Retrieve the station name to see if it's a Cimplecell, Microbase or 3rd party station.
            string currentUser = Environment.UserName;
            string microbaseStationInfo = "C:\\microbase\\tmp\\systeminfo.txt"; //"C:\\Users\\pdann\\Downloads\\systeminfo.txt"; 
            string cimplecellStationInfo = "C:\\cimplecell\\tmp\\systeminfo.txt"; //"C:\\Users\\pdann\\Downloads\\systeminfo.txt"; //

            string idletime = GetIdleTime().ToString(); // Grab the stations idle time.

            returnString.Add(idletime); //Add the idle time to our return string.

            currentUser = "microbase";
            var test = Environment.UserName;

            //Determine what feedback needs to be given and populate based on the systeminfo.txt
            switch (currentUser)
            {
                case "cimplecell":

                    //Read the file then parse out the portions we care about.
                    string[] systemInfo = await System.IO.File.ReadAllLinesAsync(cimplecellStationInfo);

                    returnString.Add(systemInfo[10]);   //CimpleCell Backup
                    returnString.Add(systemInfo[14]);   //CCInteface Version
                    returnString.Add(systemInfo[4]);    //Department
                    returnString.Add(systemInfo[15]);   //RC+ Version
                    returnString.Add(systemInfo[6]);    //EEng1
                    returnString.Add(systemInfo[7]);    //EEng2
                    returnString.Add(systemInfo[16]);   //Hostname
                    returnString.Add(systemInfo[9]);    //Language
                    returnString.Add(systemInfo[17]);   //LocalIP
                    returnString.Add(systemInfo[18]);   //MAC Address
                    returnString.Add(systemInfo[19]);   //MAC Address
                    returnString.Add(systemInfo[5]);    //Machine Name
                    returnString.Add(systemInfo[20]);   //Robot
                    returnString.Add(systemInfo[13]);   //RoofLightOnTrak
                    returnString.Add(systemInfo[11]);   //Sute
                    returnString.Add(systemInfo[3]);    //SMTP
                    returnString.Add(systemInfo[12]);   //Support Page
                    returnString.Add(systemInfo[8]);    //WebraryFTP

                    for (int i = 0; i < systemInfo.Length; i++)
                    {
                        if (systemInfo[i].Contains("ComputerSystem_CurrentTimeZone") ||
                            systemInfo[i].Contains("ComputerSystem_DaylightInEffect") ||
                            systemInfo[i].Contains("ComputerSystem_TotalPhysicalMemory") ||
                            systemInfo[i].Contains("DiskDrive_Model") ||
                            systemInfo[i].Contains("DiskDrive_SerialNumber") ||
                            systemInfo[i].Contains("DiskDrive_Size") ||
                            systemInfo[i].Contains("OperatingSystem_Name") ||
                            systemInfo[i].Contains("OperatingSystem_LastBootUpTime") ||
                            systemInfo[i].Contains("OperatingSystem_SystemDrive") ||
                            systemInfo[i].Contains("OperatingSystem_Name") ||
                            systemInfo[i].Contains("Processor_CurrentClockSpeed") ||
                            systemInfo[i].Contains("Processor_Name"))
                        {
                            returnString.Add(systemInfo[i]);
                        }
                    }

                    break;
                case "microbase":

                    //Read the file then parse out the portions we care about.
                    string[] systemInfoMicro = await System.IO.File.ReadAllLinesAsync(microbaseStationInfo);

                    for (int i = 0; i < systemInfoMicro.Length; i++)
                    {
                        if (systemInfoMicro[i].Contains("ComputerSystem_CurrentTimeZone") ||
                            systemInfoMicro[i].Contains("ComputerSystem_DaylightInEffect") ||
                            systemInfoMicro[i].Contains("ComputerSystem_TotalPhysicalMemory") ||
                            systemInfoMicro[i].Contains("DiskDrive_FreeBytes") ||
                            systemInfoMicro[i].Contains("DiskDrive_Model") ||
                            systemInfoMicro[i].Contains("DiskDrive_Size") ||
                            systemInfoMicro[i].Contains("DiskDrive_PercentFull") ||
                            systemInfoMicro[i].Contains("DiskDrive_SerialNumber") ||
                            systemInfoMicro[i].Contains("OperatingSystem_SerialNumber") || 
                            systemInfoMicro[i].Contains("Microbase_DE_Tag") ||
                            systemInfoMicro[i].Contains("Microbase_Hostname") || 
                            systemInfoMicro[i].Contains("Microbase_LocalIPAddress") ||
                            systemInfoMicro[i].Contains("OperatingSystem_Caption") ||
                            systemInfoMicro[i].Contains("OperatingSystem_LastBootUpTime") ||
                            systemInfoMicro[i].Contains("OperatingSystem_SystemDrive") ||
                            systemInfoMicro[i].Contains("Processor_CurrentClockSpeed") ||
                            systemInfoMicro[i].Contains("Processor_Name"))
                        {
                            returnString.Add(systemInfoMicro[i]);
                        }
                    }

                    break;
                default:
                    break;
            }

            return returnString;
            //return returnString; //Return a valid listing.
        }
    }
}
