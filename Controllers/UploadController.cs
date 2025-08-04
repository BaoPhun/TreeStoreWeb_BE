using AutoMapper;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TreeStore.Models.CustomModels;
using TreeStore.Models.Entities;

namespace TreeStore.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UploadController : Controller
    {
        private readonly IConfiguration _config;
        private readonly string FileFolderName = "FilesClient";
        public readonly TreeStoreDBContext _db;

        public UploadController(TreeStoreDBContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost, DisableRequestSizeLimit]
        public ResultCustomModel<UploadFileModel> Upload()
        {
            if (Request?.Form.Files.Count <= 0)
            {
                return new()
                {
                    Success = false,
                    Message = "File gửi lên không có hoặc không đúng"
                };
            }

            var file = Request.Form.Files[0];
            if (file.Length >= 1024 * 1024 * 50)
            {
                return new()
                {
                    Success = false,
                    Message = "Kích cỡ file không được vượt quá 50MB"
                };
            }

            if (file.FileName.Length >= 125)
            {
                return new()
                {
                    Success = false,
                    Message = "Tên file không được vượt quá 100 kí tự",
                };
            }

            if (!IsExtensionValid(file.FileName))
            {
                return new()
                {
                    Success = false,
                    Message = "Loại file không hợp lệ",
                };
            }

            var extension = Path.GetExtension(file.FileName);

            // path location save
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), FileFolderName);

            var now = DateTime.Now;

            var fileName = $"{file.FileName.Replace(extension, "")}__{now.Day:00}{now.Month:00}{now.Year:00}__" +
                           $"{now.Hour:00}{now.Minute:00}{now.Second:00}{now.Millisecond:000}" +
                           $"{extension}";

            // path location save + file name
            var fullPathSave = Path.Combine(pathToSave, fileName);
            //path response to client
            var pathResponseToClient = Path.Combine(FileFolderName, fileName);

            using (var stream = new FileStream(fullPathSave, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            //SaveFileToBackUp(file, fileName);

            return new()
            {
                Success = true,
                Message = "Thành công",
                Data = new()
                {
                    PathSave = pathResponseToClient,
                    OriginalName = file.FileName,
                    Url = pathResponseToClient,
                    IsImage = IsImage(file.FileName)
                }
            };
        }


        bool IsExtensionValid(string fileName)
        {
            string ext = Path.GetExtension(fileName);
            switch (ext.ToLower())
            {
                case ".pdf":
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".csv":
                case ".xlsx":
                    return true;
                default:
                    return false;
            }
        }
        bool IsImage(string fileName)
        {
            string ext = Path.GetExtension(fileName);
            switch (ext.ToLower())
            {
                case ".jpg":
                case ".jpeg":
                case ".png":
                    return true;
                default:
                    return false;
            }
        }

        public class UploadFileModel
        {
            public string PathSave { get; set; }
            public string Url { get; set; }
            public string OriginalName { get; set; }
            public bool? IsImage { get; set; }
        }
    }
}
