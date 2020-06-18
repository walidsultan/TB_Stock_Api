using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace TB_Stock.Api.FTP
{
    public class FTPHandler
    {
        private const string FTP_ROOT = "ftp://ftp.walidsultan.net/walidsultan.net/wwwroot/";

        public static string BackupDirectory(string directoryName, string username, string password)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FTP_ROOT + directoryName);
            request.Method = WebRequestMethods.Ftp.Rename;
            request.Credentials = new NetworkCredential(username, password);
            request.RenameTo = directoryName + "_" + DateTime.Now.ToString().Replace("/", "-").Replace(" ", "_").Replace(":", "-");

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                return response.StatusDescription;
            }
        }

        public static string CreateNewDirectory(string directoryName, string username, string password)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FTP_ROOT + directoryName);
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.Credentials = new NetworkCredential(username, password);

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                return response.StatusDescription;
            }
        }

        public static string UploadFile(string filename, string localFilePath, string username, string password)
        {

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FTP_ROOT + filename);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(username, password);

            request.UseBinary = true;
            request.KeepAlive = true;
            FileInfo fi = new FileInfo(localFilePath);
            request.ContentLength = fi.Length;
            byte[] buffer = new byte[4097];
            int bytes = 0;
            int total_bytes = (int)fi.Length;
            using (FileStream fs = fi.OpenRead())
            {
                using (Stream rs = request.GetRequestStream())
                {
                    while (total_bytes > 0)
                    {
                        bytes = fs.Read(buffer, 0, buffer.Length);
                        rs.Write(buffer, 0, bytes);
                        total_bytes = total_bytes - bytes;
                    }
                }
            }
            using (FtpWebResponse uploadResponse = (FtpWebResponse)request.GetResponse())
            {
                return uploadResponse.StatusDescription;
            }
        }
    }
}