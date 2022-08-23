using Microsoft.AspNetCore.Http;

namespace FileUploadTest.Models
{
    public class FileModel
    {
        public IFormFile File { get; set; }
    }
}
