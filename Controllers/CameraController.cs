using I_Attend.Data;
using I_Attend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;

namespace I_Attend.Controllers
{
    public class CameraController : Controller
    {
        private readonly I_AttendDAO _context;
        private readonly ILogger<CameraController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CameraController(I_AttendDAO context, ILogger<CameraController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        }

        //[Authorize(Roles = "User")]
        public IActionResult Capture()
        {
            return View();
        }

        //[Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> Capture(string matricNumber)
        {
            try
            {
                var files = HttpContext.Request.Form.Files;
                if (files != null && files.Count > 0 && !string.IsNullOrEmpty(matricNumber))
                {
                    // Verify matric number exists
                    var users = await _context.GetViewsAsync();
                    if (!users.Any(u => u.Matric_Number == matricNumber))
                    {
                        return Json(new { success = false, message = "Matric number not found in database" });
                    }

                    var file = files[0];
                    if (file.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream);
                            var imageBytes = memoryStream.ToArray();

                            // Validate image format
                            if (!IsValidImage(imageBytes))
                            {
                                return Json(new { success = false, message = "Invalid image format. Please upload a JPEG or PNG image." });
                            }

                            // Store in database
                            var success = await _context.AddStudentImageAsync(matricNumber, imageBytes);

                            if (success)
                            {
                                // Optionally store in folder
                                var fileName = $"{Guid.NewGuid()}_{matricNumber}{Path.GetExtension(file.FileName)}";
                                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "CameraPhotos", fileName);
                                Directory.CreateDirectory(Path.GetDirectoryName(filePath)); // Ensure directory exists
                                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                                return Json(new { success = true, message = "Image captured/uploaded successfully" });
                            }
                        }
                    }
                }
                return Json(new { success = false, message = "Invalid file or matric number" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error capturing/uploading image for matric number {MatricNumber}", matricNumber);
                return Json(new { success = false, message = "Error processing image" });
            }
        }

        public async Task<IActionResult> Profile()
        {
            var matricNumber = User?.FindFirst("MatricNumber")?.Value; // Use custom claim
            if (string.IsNullOrEmpty(matricNumber))
            {
                return RedirectToAction("Login", "Views"); // Updated to match ViewsController
            }

            var users = await _context.GetViewsAsync();
            var view = users.FirstOrDefault(v => v.Matric_Number == matricNumber);

            if (view == null)
            {
                ViewBag.Message = "Student profile not found";
                return View(new View());
            }

            if (view.ImageData == null)
            {
                ViewBag.Message = "No profile image found";
            }
            else
            {
                ViewBag.ImageData = Convert.ToBase64String(view.ImageData);
            }

            return View("~/Views/Camera/Profile.cshtml", view);
        }

        private bool IsValidImage(byte[] bytes)
        {
            try
            {
                using (var stream = new MemoryStream(bytes))
                {
                    IImageFormat format = Image.DetectFormat(stream);
                    return format is JpegFormat || format is PngFormat;

                }
            }
            catch
            {
                return false;
            }
        }

        //Just in case SixLabors.ImageSharp isn't compatible globally "with other platforms" use this:
        //private bool IsValidImage(byte[] bytes)
        //{
        //    if (bytes == null || bytes.Length < 4)
        //        return false;

        //    try
        //    {
        //        // JPEG: Starts with 0xFF, 0xD8 (SOI marker)
        //        bool isJpeg = bytes[0] == 0xFF && bytes[1] == 0xD8;

        //        // PNG: Starts with 0x89, 0x50, 0x4E, 0x47 (PNG signature)
        //        bool isPng = bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47;

        //        // Return true if the bytes match JPEG or PNG signatures
        //        return isJpeg || isPng;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
    }
}











//using Microsoft.AspNetCore.Mvc;
//using I_Attend.Models;
//using I_Attend.Data;
//using System.IO;

//namespace I_Attend.Controllers
//{
//    public class CameraController : Controller
//    {
//        private readonly I_AttendDAO _context;
//        private readonly ILogger<CameraController> _logger;
//        private readonly IWebHostEnvironment _webHostEnvironment;

//        public CameraController(I_AttendDAO context, ILogger<CameraController> logger, IWebHostEnvironment webHostEnvironment)
//        {
//            _context = context ?? throw new ArgumentNullException(nameof(context));
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
//        }

//        public IActionResult Capture()
//        {
//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> Capture(string matricNumber)
//        {
//            try
//            {
//                var files = HttpContext.Request.Form.Files;
//                if (files != null && files.Count > 0 && !string.IsNullOrEmpty(matricNumber))
//                {
//                    var file = files[0];
//                    if (file.Length > 0)
//                    {
//                        using (var memoryStream = new MemoryStream())
//                        {
//                            await file.CopyToAsync(memoryStream);
//                            var imageBytes = memoryStream.ToArray();

//                            // Validate image format
//                            if (!IsValidImage(imageBytes))
//                            {
//                                return Json(new { success = false, message = "Invalid image format. Please upload a JPEG or PNG image." });
//                            }

//                            // Store in database
//                            var success = await _context.AddStudentImageAsync(matricNumber, imageBytes);

//                            if (success)
//                            {
//                                // Optionally store in folder
//                                var fileName = $"{Guid.NewGuid()}_{matricNumber}{Path.GetExtension(file.FileName)}";
//                                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "CameraPhotos", fileName);
//                                Directory.CreateDirectory(Path.GetDirectoryName(filePath)); // Ensure directory exists
//                                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

//                                return Json(new { success = true, message = "Image captured/uploaded successfully" });
//                            }
//                        }
//                    }
//                }
//                return Json(new { success = false, message = "Invalid file or matric number" });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error capturing/uploading image for matric number {MatricNumber}", matricNumber);
//                return Json(new { success = false, message = "Error processing image" });
//            }
//        }

//        public async Task<IActionResult> Profile()
//        {
//            var matricNumber = User?.Identity?.Name; // Adjust based on your authentication setup
//            if (string.IsNullOrEmpty(matricNumber))
//            {
//                return RedirectToAction("Login", "Account"); // Adjust controller/action as needed
//            }

//            var user = await _context.GetViewsAsync();
//            var view = user.FirstOrDefault(v => v.Matric_Number == matricNumber);

//            if (view == null || view.ImageData == null)
//            {
//                ViewBag.Message = "No profile image found";
//                return View(new View());
//            }

//            ViewBag.ImageData = Convert.ToBase64String(view.ImageData);
//            return View(view);
//        }

//        private bool IsValidImage(byte[] bytes)
//        {
//            try
//            {
//                using (var stream = new MemoryStream(bytes))
//                using (var image = System.Drawing.Image.FromStream(stream))
//                {
//                    return image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg) ||
//                           image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png);
//                }
//            }
//            catch
//            {
//                return false;
//            }
//        }
//    }
//}























//using Microsoft.AspNetCore.Mvc;
//using I_Attend.Models;
//using I_Attend.Data;


//namespace I_Attend.Controllers
//{
//    public class CameraController : Controller
//    {
//        private readonly I_AttendDAO _context;
//        private readonly ILogger<CameraController> _logger;
//        private readonly IWebHostEnvironment _webHostEnvironment;

//        public CameraController(IWebHostEnvironment webHostEnvironment)
//        {
//            _webHostEnvironment = webHostEnvironment;
//        }
//        public IActionResult Capture()
//        {
//            return View();
//        }
//        [HttpPost]
//        public IActionResult Capture(string name)
//        {
//            try
//            {
//                var files = HttpContext.Request.Form.Files;
//                if (files != null)
//                {
//                    foreach (var file in files)
//                    {
//                        if (file.Length > 0)
//                        {
//                            var fileName = file.FileName;
//                            var myUniqueFileName = Convert.ToString(Guid.NewGuid());
//                            var fileExtension = Path.GetExtension(fileName);
//                            var newFileName = string.Concat(myUniqueFileName, fileExtension);
//                            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "CameraPhotos") + $@"\{newFileName}";
//                            if (!string.IsNullOrEmpty(filePath))
//                            {
//                                StoreInFolder(file, filePath);
//                            }
//                            var imageBytes = System.IO.File.ReadAllBytes(filePath);
//                            if(imageBytes != null)
//                            {
//                                StoreInDatabase(imageBytes);
//                            }
//                        }
//                    }
//                    return Json(true);
//                }
//                else
//                {
//                    return Json(false);
//                }
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//        }
//        private void StoreInFolder(IFormFile file,  string fileName)
//        {
//            using (FileStream fs = System.IO.File.Create(fileName))
//            {
//                file.CopyTo(fs);
//                fs.Flush();

//            }
//        }
//        private void StoreInDatabase(byte[] imageBytes)
//        {

//        }

//    }
//}
