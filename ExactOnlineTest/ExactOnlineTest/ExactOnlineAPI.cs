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

namespace ExactOnlineTest
{
    class ExactOnlineAPI
    {
        private static Guid GeneralCategory = Guid.Parse("3b6d3833-b31b-423d-bc3c-39c62b8f2b12");
        private static int MiscDocType = 55;

        public ExactOnlineAPI()
        {

        }

        public void ConnectToExactOnline2Upload(string srcFolder, string srcFilename)
        {
            try
            {
                // To make this work set the authorisation properties of your test app in the testapp.config.
                var testApp = new TestApp();

                var connector = new Connector(testApp.ClientId.ToString(), testApp.ClientSecret, testApp.CallbackUrl);
                var client = new ExactOnlineClient(connector.EndPoint, connector.GetAccessToken);
                 
                //Create document which to relate documentattachment to
                ExactOnline.Client.Models.Documents.Document document = new ExactOnline.Client.Models.Documents.Document
                {
                    Subject = Path.GetFileNameWithoutExtension(srcFilename),
                    Body = Path.GetFileNameWithoutExtension(srcFilename),
                    Category = GeneralCategory,
                    Type = MiscDocType, 
                    DocumentDate = DateTime.Now.Date
                };

                bool created = client.For<ExactOnline.Client.Models.Documents.Document>().Insert(ref document);

                if (created)
                {
                    //convert attachment into byte array
                    byte[] attachment = System.IO.File.ReadAllBytes(srcFolder + "/" + srcFilename);

                    //Create DocumentAttachment
                    DocumentAttachment documentAttachment = new DocumentAttachment
                    {
                        Document = document.ID, //relate DocumentAttachment to previously created document
                        Attachment = attachment,
                        FileName = srcFilename
                    };

                    created = client.For<DocumentAttachment>().Insert(ref documentAttachment);
                    Console.WriteLine("Saved to ExactOnline {0} attachmentID {1}", srcFilename, documentAttachment.ID);
                }
                else
                {
                    Console.WriteLine("Unable to create document");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured: {0}", ex.Message);
            }
        }
    }
}
