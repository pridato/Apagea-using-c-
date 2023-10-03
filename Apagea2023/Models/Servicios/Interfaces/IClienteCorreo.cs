using System;
namespace Apagea2023.Models.Servicios.Interfaces
{
	public interface IClienteCorreo
	{
        #region ... propiedades interface ICliente ...

        public String UserId { get; set; }
        public String Key { get; set; }

        #endregion

        #region ... metodos interface ICliente ...

        public bool EnviarEmail(String emailcliente, String subject, String cuerpoMensaje, String? ficheroAdjunto);

        #endregion
    }
}

