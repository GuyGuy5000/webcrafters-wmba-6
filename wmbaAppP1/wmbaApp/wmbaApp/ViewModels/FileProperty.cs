using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace wmbaApp.ViewModels
{
    public class FileProperty
    {
        [Key, ForeignKey("UploadedFile")]
        public int FilePropertyID { get; set; }

        [ScaffoldColumn(false)]
        public byte[] Property { get; set; }

        public UploadedFile UploadedFile { get; set; }
    }
}
