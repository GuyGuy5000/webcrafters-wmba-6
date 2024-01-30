using System.ComponentModel.DataAnnotations;
using wmbaApp.Models;

namespace wmbaApp.ViewModels
{
    public class PlayerThumbnail
    {
        public int ID { get; set; }

        [ScaffoldColumn(false)]
        public byte[] Content { get; set; }

        [StringLength(255)]
        public string MimeType { get; set; }

        public int PlayerID { get; set; }
        public Player Player { get; set; }
    }
}
