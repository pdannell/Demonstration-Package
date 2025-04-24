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

namespace Station_Sentinal.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DesktopImageController : Controller
    {
    
        [HttpGet(Name = "DesktopImage")]
        public async Task<string> FilePathes() {

            //Initalize our Byte Array For Feedback.
            Byte[] fileBytes;
            string returnString = "";

            //Grab an image of our PC Currently.
            int screenWidth = 1920;
            int screenHeight = 1080;

            //Default save location for our PC's image.
            string fileLocation = "C:\\PCImage.jpg";

            Bitmap bmp = new(screenWidth, screenHeight);        //Create Bitmap to our screen size.
            Size screenSize = new(screenWidth, screenHeight);   //Set the screen size
            float scaleFactor = .55f;                           //Scale factor for resizing output.
            
            //Save screen contents and resize our Bitmap for readability.
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(0, 0, 0, 0, screenSize);

                #region "The commented out section is if we need a higher quality bitmap image than we're getting"
                //g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                //g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
                //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                //using (var wrapMode = new ImageAttributes())
                //{
                //    wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                //    g.DrawImage(bmp, 0, 0, screensize.Width / 2, screensize.Height / 2);
                //}
                //bmpResized = new Bitmap(bmp, screensize / 2);
                //bmpResized.Save("C:\\PCImage2.jpg");  // saves the image
                #endregion

                bmp = new Bitmap(bmp, (int)(screenSize.Width * scaleFactor), (int)(screenSize.Height * scaleFactor));
                bmp.Save("C:\\PCImage.jpg");  // saves the image
            }

            try //Read the file
            {
                fileBytes = System.IO.File.ReadAllBytes(fileLocation);

                returnString = Convert.ToBase64String(fileBytes);
            }
            catch(Exception)    //Some sort of failure occured most likely due to a non existant file.
            {
                return returnString;
            }

            await Task.Delay(100);

            return returnString; //Return a valid listing.
        }
    }
}
