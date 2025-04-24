using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Extensions.Configuration;
using System.Reflection.Metadata;
using System;

namespace Station_Sentinal.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DownloadController : Controller
    {
    
        [HttpGet(Name = "Download")]
        public async Task<Byte[]> FilePathes(String fileLocation) {

            //Initalize our Byte Array For Feedback.
            Byte[] fileBytes = [];

            try //Read the file.
            {
                fileBytes = System.IO.File.ReadAllBytes(fileLocation);
            }
            catch(Exception)    //Some sort of failure occured most likely due to a non existant file.
            {
                return fileBytes;
            }

            await Task.Delay(1);

            return fileBytes; //Return a valid listing.
        }
    }
}
