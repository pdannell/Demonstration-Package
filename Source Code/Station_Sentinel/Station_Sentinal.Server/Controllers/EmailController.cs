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
    public class EmailController : Controller
    {

        public SmtpClient smtpClient = new("mail.smtp2go.com", 80);
        
        [HttpGet(Name = "SendSupportEmail")]
        public async void SendSupportemail(String firstName , String lastName, String emailAddress, String bodyText ) {

            //Populate the message to send
            MailMessage message = new("pdannelly@borgwarner.com", "iguyer@borgwarner.com", "Support Request",
                "First Name: " + firstName +
                "     Last Name" + lastName + ("\n") +
                "Email Address: " + emailAddress + "\n" +
                "Request: " + bodyText)
            {
                IsBodyHtml = true,
                Priority = MailPriority.Normal
            };

            //Send Message
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential("StationController", "jQYI6hWUWoizg5HU");
            //smtpClient.Timeout = 2000;
            try
            {
                //smtpClient.Send(message);
                await smtpClient.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText("C:\\StationController.txt",ex.ToString());
            }
        }
    }
}
