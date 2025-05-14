using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KendoCRUDService.Models
{
    public class FormUploadItemsViewModel
    {
        public string TextBox { get; set; }
        public string TextArea { get; set; }
        public int NumericTextBox { get; set; }
        public string MaskedTextBox { get; set; }
        public DateTime DatePicker { get; set; }
        public DateTime DateTimePicker { get; set; }
        public string RadioGroup { get; set; }
        public List<string> CheckBoxGroup { get; set; }
        public bool Switch { get; set; }
        public int ComboBox { get; set; }
        public int DropDownList { get; set; }
        public IFormFile FileName { get; set; }
    }
}
