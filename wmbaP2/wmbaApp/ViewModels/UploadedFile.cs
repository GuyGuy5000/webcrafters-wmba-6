using System.ComponentModel.DataAnnotations;

namespace wmbaApp.ViewModels
{
    public class UploadedFile
    {
        public int ID { get; set; }

        [StringLength(255, ErrorMessage = "The name of the file cannot be more than 255 characters.")]
        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [StringLength(255)]
        public string MimeType { get; set; }

        public FileProperty FileProperty { get; set; } = new FileProperty();
    }
}
