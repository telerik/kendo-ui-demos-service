using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.IO;
using KendoCRUDService.Models;

namespace KendoCRUDService.Controllers
{
    public class FormUploadController : Controller
    {
        public FormUploadController()
        {
            
        }

        [HttpPost]
        public async Task<IActionResult> Upload(FormUploadViewModel model)
        {
            // Validate inputs
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.Email) || model.File == null)
            {
                return BadRequest(new { Success = false, Message = "All fields are required" });
            }

            try
            {
                // Process the file
                var fileName = Path.GetFileName(model.File.FileName);
                var fileSize = model.File.Length;
                
                // You can save the file if needed
                // var filePath = Path.Combine("YourUploadDirectory", fileName);
                // using (var stream = new FileStream(filePath, FileMode.Create))
                // {
                //     await file.CopyToAsync(stream);
                // }

                // Return success response
                return Json(new { 
                    Success = true, 
                    Message = "Data received successfully", 
                    UserData = new {
                        model.Username,
                        model.Email
                    }, 
                    FileData = new { 
                        FileName = fileName, 
                        FileSize = fileSize 
                    } 
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("formupload/uploadformitems")]
        public async Task<IActionResult> UploadFormItems(FormUploadItemsViewModel model)
        {
            // Validate inputs (adjust validation as needed for your required fields)
            if (string.IsNullOrEmpty(model.TextBox) || model.FileName == null)
            {
                return BadRequest(new { Success = false, Message = "Required fields are missing" });
            }

            try
            {
                // Process the file
                var fileName = Path.GetFileName(model.FileName.FileName);
                var fileSize = model.FileName.Length;

                // You can save the file if needed
                // var filePath = Path.Combine("YourUploadDirectory", fileName);
                // using (var stream = new FileStream(filePath, FileMode.Create))
                // {
                //     await model.FileName.CopyToAsync(stream);
                // }

                // Return success response
                return Json(new
                {
                    Success = true,
                    Message = "Data received successfully",
                    FormData = new
                    {
                        model.TextBox,
                        model.TextArea,
                        model.NumericTextBox,
                        model.MaskedTextBox,
                        model.DatePicker,
                        model.DateTimePicker,
                        model.RadioGroup,
                        model.CheckBoxGroup,
                        model.Switch,
                        model.ComboBox,
                        model.DropDownList
                    },
                    FileData = new
                    {
                        FileName = fileName,
                        FileSize = fileSize
                    }
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
    }
}
