using System;
using System.Drawing;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.MimeTypes;
using Abp.Runtime.Security;
using Hatra.Messenger.Controllers;
using Hatra.Messenger.Models.File;
using Hatra.Messenger.Net.MimeTypes;
using Hatra.Messenger.SettingModels;
using Hatra.Messenger.Tools;
using Hatra.Messenger.Web.Host.Hubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Hatra.Messenger.Web.Host.Controllers
{

    [AbpAuthorize]
    public class UploaderController : MessengerControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly UploadSetting _uploadSetting;

        public UploaderController(IWebHostEnvironment hostingEnvironment, IHubContext<ChatHub> hubContext, UploadSetting uploadSetting)
        {
            _hostingEnvironment = hostingEnvironment;
            _hubContext = hubContext;
            _uploadSetting = uploadSetting;
        }

        [HttpPost]
        [Route("api/Upload/{uploadKey}")]
        public async Task<ActionResult<UploadResultModel>> Upload(string uploadKey, [FromForm] UploadModel model)
        {
            var totalBytes = model.File.Length;
            if (totalBytes < 512)
            {
                return BadRequest();
            }
            if (totalBytes > _uploadSetting.AllowedFileSize * 1024 * 1024)
            {
                return BadRequest($"حجم فایل نباید از {_uploadSetting.AllowedFileSize} مگابایت بیشتر باشد");
            }

            var filename = Guid.NewGuid().ToString("N");
            var extension = Path.GetExtension(model.File.FileName);
            filename = EnsureCorrectFilename(filename + extension);
            var thumbName = string.Empty;
            var buffer = new byte[_uploadSetting.BufferSize * 1024];
            using (var output = System.IO.File.Create(GetPathAndFilename(filename)))
            {
                using (var input = model.File.OpenReadStream())
                {
                    long totalReadBytes = 0;
                    int readBytes;

                    while ((readBytes = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await output.WriteAsync(buffer, 0, readBytes);
                        totalReadBytes += readBytes;
                        await PushUploadProgressPercentToClient(User.Identity.GetUserId().Value, uploadKey, (int)(totalReadBytes / (float)totalBytes * 100.0));
                    }
                }

                if (model.File.IsImage())
                {
                    thumbName = Path.ChangeExtension(filename, "thumb");
                    CreateThumbnail(output, thumbName, _uploadSetting.ThumbnailWidth, _uploadSetting.ThumbnailHeight);
                }
            }

            return new ActionResult<UploadResultModel>(new UploadResultModel(filename, thumbName));
        }

        private async Task PushUploadProgressPercentToClient(long userId, string uploadKey, int percent)
        {
            if (ChatHub.OnlineUsers.TryGetValue(userId, out var connectionId))
                await _hubContext.Clients.Client(connectionId).PushUploadProgressPercentAsync(uploadKey, percent);
        }

        private string EnsureCorrectFilename(string filename)
        {
            if (filename.Contains("\\"))
                filename = filename.Substring(filename.LastIndexOf("\\", StringComparison.Ordinal) + 1);

            return filename;
        }

        private string GetPathAndFilename(string filename)
        {
            var path = _hostingEnvironment.WebRootPath + $"\\{_uploadSetting.UploadDirectory}\\";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path + filename;
        }

        private void CreateThumbnail(Stream fileStream, string fileName, int desiredWidth = 250, int desiredHeight = 250)
        {
            try
            {
                var uploadedImage = Image.FromStream(fileStream);
                var thumb = uploadedImage.GetThumbnailImage(desiredWidth, desiredHeight, () => false, IntPtr.Zero);
                thumb.Save(GetPathAndFilename(fileName));
            }
            catch (Exception)
            {
                //Ignored
            }
        }
    }
}
