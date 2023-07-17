using System.ComponentModel.DataAnnotations;

namespace SigeaciInterface.Models
{
    public class Documentos
    {
        [Required(ErrorMessage= "El archivo es obligatorio")]
        public List<Documento> Files { get; set; }
    }
}
