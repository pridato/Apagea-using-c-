using System;
namespace Apagea2023.Models
{
	public class Libro
	{
        #region ... propiedades clase Libro ...

        public String IdCategoria { get; set; } = "";

        public String Titulo { get; set; } = "";

        public String Editorial { get; set; } = "";

        public String Autores { get; set; } = "";


        public String Edicion { get; set; } = "";

        public String Dimensiones { get; set; } = "";

        public String Idioma { get; set; } = "";

        public String ISBN13 { get; set; } = "";

        public String ISBN10 { get; set; } = "";

        public String Resumen { get; set; } = "";

        public int NumeroPaginas { get; set; } = 0;

        public Decimal Precio { get; set; } = 0;
        public string ImagenLibroBASE64 { get; internal set; } = "";


        #endregion

        #region ... metodos clase Libro ...

        #endregion
    }
}

