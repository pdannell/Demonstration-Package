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
    public class SystemNameController : Controller
    {

        [HttpGet(Name = "SystemName")]
        public async Task<string> SystemName() {
            await Task.Delay(1);
            return Environment.MachineName;
        }
    }
}
