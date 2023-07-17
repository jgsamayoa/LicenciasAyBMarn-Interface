using System.ComponentModel.DataAnnotations;

namespace SigeaciInterface.Models
{
    public class Documento
    {
        [Required(ErrorMessage = "Elija un documento: .docx, .xlsx, .pdf")]
        public IFormFile Files { get; set; }

        [Required(ErrorMessage = "Debe definir un path para el almacenamiento")]
        public string path { get; set; }
    }
}
