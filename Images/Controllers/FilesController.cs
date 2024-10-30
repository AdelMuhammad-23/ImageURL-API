using Images.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Images.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        [HttpPost("Upload")]
        public ActionResult<UploadFilesDTO> UploadFile(IFormFile file)
        {
            #region Check Extensions
            var extension = Path.GetExtension(file.FileName);
            var allowExtensions = new string[]
            {
                ".PNG",
                ".JPG",
                ".SVG"
            };
            var extensionComparer = allowExtensions.Contains(extension, StringComparer.InvariantCultureIgnoreCase);
            if (!extensionComparer)
                return BadRequest(new UploadFilesDTO(false, "this extension is not allowed"));
            #endregion

            #region Check Size
            var allowSize = file.Length is > 0 and < 4_000_000;
            if (!allowSize) return BadRequest(new UploadFilesDTO(false, "this file is too large"));
            #endregion

            #region Storage Image
            var newFileName = $"{Guid.NewGuid()}{extension}";
            var pathName = Path.Combine(Environment.CurrentDirectory, "Images");
            var fullFileName = Path.Combine(pathName, newFileName);

            using var stream = new FileStream(fullFileName, FileMode.Create);
            file.CopyTo(stream);
            #endregion

            #region Generate URL
            var url = $"{Request.Scheme}://{Request.Host}/Images/{fullFileName}";
            return Ok(new UploadFilesDTO(true, "Success", url));
            #endregion
        }
    }
}
