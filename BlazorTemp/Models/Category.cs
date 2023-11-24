using System.ComponentModel.DataAnnotations;

namespace BlazorTemp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [MaxLength(100, ErrorMessage = "Insira no máximo 100 caracteres")]
        [MinLength(3, ErrorMessage = "Insira no mínimo 3 caracteres")]
        public string Title { get; set; } = String.Empty;
    }
}
