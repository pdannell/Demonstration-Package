using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Extensions.Configuration;

namespace Station_Sentinal.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileExplorerController : Controller
    {

        //Return string for all our files/folders that are seen.
        private readonly List<string> fileListing = [];
        
        [HttpGet(Name = "FileExplorer")]
        public async Task<List<string>> FilePathes(String folderLocation) {
            try
            {
                //Add all directories that were seen.
                foreach (String file in System.IO.Directory.GetDirectories(folderLocation))
                {
                    fileListing.Add(file);
                }

                //Add all files that were seen with their extension.
                foreach (String file in System.IO.Directory.GetFiles(folderLocation))
                {
                    fileListing.Add(file);
                }
            }
            catch(Exception)    //Some sort of failure occured most likely due to an invalid address.
            {
                fileListing.Add("Invalid FilePath Entered");
                return fileListing; //Return an error as the user entered something wrong?
            }

            await Task.Delay(1);

            return fileListing; //Return a valid listing.
        }
    }
}
