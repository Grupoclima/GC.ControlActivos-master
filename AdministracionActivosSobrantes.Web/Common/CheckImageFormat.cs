using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdministracionActivosSobrantes.Web.Common
{
    public static class CheckImageFormat
    {

        public static bool IsImage(HttpPostedFile file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }

            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" ,".pdf",".pdf"}; // add more if u like...

            // linq from Henrik Stenbæk
            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }
    }
}