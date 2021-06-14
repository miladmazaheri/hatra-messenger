using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Hatra.Messenger.Models.File
{
    public class UploadModel
    {
        [Required]
        public IFormFile File { get; set; }
        [Required]
        public Guid MediaId { get; set; }
    }
}
