namespace KendoCRUDService.Models
{
    public class FormUploadViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public IFormFile File { get; set; }
    }
}
