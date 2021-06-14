using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatra.Messenger.SettingModels
{
    public class UploadSetting
    {
        public int AllowedFileSize { get; set; } = 50;
        public int BufferSize { get; set; } = 16;
        public string UploadDirectory { get; set; } = "Upload";
        public int ThumbnailWidth { get; set; } = 250;
        public int ThumbnailHeight { get; set; } = 250;
    }
}
