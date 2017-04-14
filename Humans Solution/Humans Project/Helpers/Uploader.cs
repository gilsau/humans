using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Humans.Web.Models;

namespace Humans.Web.Helpers
{
    public static class Uploader
    {
        public static void UploadFile(HttpPostedFileBase file, string folder, out Result result)
        {
            result = new Result();

            string[] fileParts = file.FileName.Split('.');
            string extension = fileParts[fileParts.Length - 1];
            string filenameRoot = Guid.NewGuid().ToString();
            string filename = string.Format("{0}.{1}", filenameRoot, extension).ToLower();

            try
            {
                file.SaveAs(HttpContext.Current.Server.MapPath(string.Format("~/{0}/{1}", folder, filename)));
                result.DynObject = filename;
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.MessageForUser = "There was an error uploading the file.";
                result.MessageForLog = ex.Message;
                Logger.LogError(ex.Message);

                throw ex;
            }
        }
    }
}