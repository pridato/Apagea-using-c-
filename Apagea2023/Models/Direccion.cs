using System;
namespace Apagea2023.Models
{
	public class Direccion
	{
        #region ... propiedades ...

        public String IdDireccion { get; set; } = Guid.NewGuid().ToString();
        public String Calle { get; set; } = "";
        public int CP { get; set; } = 0;

        public Provincia ProvinciaDirecc { get; set; } = new Provincia();
        public Municipio MunicipioDirec { get; set; } = new Municipio();

        public String Pais { get; set; } = "España";
        public Boolean EsPrincipal { get; set; } = false;
        public Boolean EsFacturacion { get; set; } = false;

        #endregion

        #region ... metodos ...

        #endregion
    }
}

