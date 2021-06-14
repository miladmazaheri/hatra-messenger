using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Hatra.Messenger.Tools
{
    public static class HttpPostedFileBaseExtensions
    {
        public const int ImageMinimumBytes = 512;

        public static bool IsImage(this IFormFile postedFile)
        {
            //if (!string.Equals(postedFile.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) &&
            //    !string.Equals(postedFile.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) &&
            //    !string.Equals(postedFile.ContentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) &&
            //    !string.Equals(postedFile.ContentType, "image/gif", StringComparison.OrdinalIgnoreCase) &&
            //    !string.Equals(postedFile.ContentType, "image/x-png", StringComparison.OrdinalIgnoreCase) &&
            //    !string.Equals(postedFile.ContentType, "image/png", StringComparison.OrdinalIgnoreCase))
            //{
            //    return false;
            //}

            var postedFileExtension = Path.GetExtension(postedFile.FileName);
            return string.Equals(postedFileExtension, ".jpg", StringComparison.OrdinalIgnoreCase) || string.Equals(postedFileExtension, ".png", StringComparison.OrdinalIgnoreCase) || string.Equals(postedFileExtension, ".gif", StringComparison.OrdinalIgnoreCase) || string.Equals(postedFileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase);
        }
    }
}
