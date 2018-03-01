using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Dropbox.Api;
using Dropbox.Api.Files;

namespace ExactOnlineTest
{
    class DropboxAPI
    {
        private static DropboxClient _client;
        public DropboxAPI(string accessToken)
        {
            _client = new DropboxClient(accessToken);
        } 

        public async Task<int> ConnectToDropbox2Upload(string destFolder, string srcFolder, string srcFilename)
        {
            int success = 1;
            try
            {
                //var accessToken = "1iRSfOKEAI4AAAAAAAAC0hQd0zCoFvtv-n4vMXu6C_qCbqTDDc7m62LMrJqiooqT";
                //var client = new DropboxClient(accessToken);

                success = await UploadFile(_client, destFolder, srcFilename, srcFolder, srcFilename);
            }
            catch(Exception ex)
            {
                success = 0;
                Console.WriteLine("Error Occurred during connection to account: {0}", ex.Message);
            }
            return success;
        }

        public async Task ListRootFolder(DropboxClient dbx)
        {
            var list = await dbx.Files.ListFolderAsync(string.Empty);

            // show folders then files
            foreach (var item in list.Entries.Where(i => i.IsFolder))
            {
                Console.WriteLine("D  {0}/", item.Name);
            }

            foreach (var item in list.Entries.Where(i => i.IsFile))
            {
                Console.WriteLine("F{0,8} {1}", item.AsFile.Size, item.Name);
            }

        }

        public async Task UploadTextFile(DropboxClient dbx, string folder, string file, string content)
        {
            using (var mem = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                try
                {
                    var updated = await dbx.Files.UploadAsync(
                        folder + "/" + file,
                        WriteMode.Overwrite.Instance,
                        body: mem);
                    Console.WriteLine("Saved {0}/{1} rev {2}", folder, file, updated.Rev);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public async Task<int> UploadFile(DropboxClient dbx, string destFolder, string destFile, string srcFolder, string srcFile)
        {
            int success = 1;

            try
            {
                using (var fileStream = File.Open(srcFolder + "/" + srcFile, FileMode.Open))
                {
                    try
                    {
                        var updated = await dbx.Files.UploadAsync(
                            destFolder + "/" + destFile,
                            WriteMode.Overwrite.Instance,
                            body: fileStream);
                        Console.WriteLine("Saved to DropBox {0}/{1} rev {2}", destFolder, destFile, updated.Rev);
                    }
                    catch (Exception ex)
                    {
                        success = 0;
                        Console.WriteLine("Error Occurred during reading of file: {0}", ex.Message);
                    }
                }
            }
            catch(Exception ex)
            {
                success = 0;
                Console.WriteLine("Error Occurred during reading of file: {0}", ex.Message);
            }

            return success;
        }
    }
}
