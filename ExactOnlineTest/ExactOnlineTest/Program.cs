using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dropbox.Api;
using ExactOnline.Client.Models;
using ExactOnline.Client.Models.Documents;
using ExactOnline.Client.Models.CRM;
using ExactOnline.Client.Sdk;
using ExactOnline.Client.Sdk.Controllers;
using ExactOnline.Client.Sdk.Helpers;
using System.IO;
using Dropbox.Api.Files;
using System.Configuration;

namespace ExactOnlineTest
{
    class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            Console.WriteLine("Press any key to begin upload to Dropbox and to ExactOnline");
            Console.ReadKey();

            try
            {
                string destFolder = @"/files";
                string srcFolder = @"\files";
                string srcFilename = "FunkoBatman.jpg";
                string accessToken = ConfigurationSettings.AppSettings["DropBoxAccessToken"] ;

                DropboxAPI dropboxAPI = new DropboxAPI(accessToken);
                //Connect to Dropbox and upload file
                var task = Task.Run(() => dropboxAPI.ConnectToDropbox2Upload(destFolder, srcFolder, srcFilename));

                task.Wait();
                //If Dropbox upload is successful, continue with uploading to ExactOnline account
                if (task.Result == 1)
                {
                    ExactOnlineAPI exactOnlineAPI = new ExactOnlineAPI();

                    exactOnlineAPI.ConnectToExactOnline2Upload(srcFolder, srcFilename);
                }
                else
                    Console.WriteLine("Unable to upload file into Dropbox account");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error occurred: {0}", ex.Message);
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            return 0;
        }

        

        private static Guid GetCategoryId(ExactOnlineClient client)
        {
            var categories = client.For<DocumentCategory>().Select("ID").Where("Description+eq+'General'").Get();
            var category = categories.First().ID;
            return category;
        }
    }
}
