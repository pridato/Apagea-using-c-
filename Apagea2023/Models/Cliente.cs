using System.ComponentModel.DataAnnotations;
namespace Apagea2023.Models
{
	public class Cliente
	{
        #region ... propiedades Modelo Cliente ...

        public String IdCliente { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage ="*El nombre es obligatorio")]
        [MaxLength(50, ErrorMessage = "* no se admiten mas de 50 caracteres en el nombre")]
        public String Nombre { get; set; } = "";

        [Required(ErrorMessage = "* los Apellidos son obligatorios")]
        [MaxLength(200, ErrorMessage = "* no se admiten mas de 200 caracteres en los apellidos")]
        public String Apellidos { get; set; } = "";

        [Required(ErrorMessage = "* el Telefono de contacto es obligatorio")]
        [RegularExpression(@"^\d{3}(\s?\d{2}){3}$", ErrorMessage = "* formato de Telefono invalido: 666 11 22 33")]
        public String Telefono { get; set; } = "";

        public Cuenta Credenciales { get; set; } = new Cuenta();

        #endregion

        #region ... metodos Modelo Cliente ...

        #endregion

    }
}

