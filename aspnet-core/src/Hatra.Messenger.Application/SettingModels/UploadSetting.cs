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
        public string ThumbnailPrefix { get; set; } = "t_";
    }
}
