using Apagea2023.Models.Servicios.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Apagea2023.Models.Servicios
{
	public class MailJetServices : IClienteCorreo
	{
        #region ... Constructor modelo servicio envío email ...
        public MailJetServices(IConfiguration servicioAccesoAppSettingsDI)
        {
            this.__accesoAppSettings = servicioAccesoAppSettingsDI;

            this.UserId = this.__accesoAppSettings.GetSection("MailJetCredentials:ClaveAPI").Value;
            this.Key = this.__accesoAppSettings.GetSection("MailJetCredentials:ClaveSecreta").Value;
        }
        #endregion ...

        #region ... propiedades del modelo servicio envío email ...

        private IConfiguration __accesoAppSettings;

        public string UserId { get; set; } //= "aasfsadfsdaf";

        public string Key { get; set; } //="fsfsaf;"

        #endregion

        #region ... metodos del modelo servicio envío email ...

        public bool EnviarEmail(string emailcliente, string subject, string cuerpoMensaje, string? ficheroAdjunto)
        {
            try
            {
                //1º: abrir socket (conexion) al servidor SMTP de mailjet con las credenciales de la api q te dan 
                //cuando te registras <=== usando la clase SmtpClient
                SmtpClient _clienteSMTP = new("in-v3.mailjet.com")
                {
                    Credentials = new NetworkCredential(this.UserId, this.Key)
                };

                //2º: crear el cuerpo del mensaje de correo a mandar al cliente.. <=== clase MailMessage
                MailMessage _mensajeAEnviar = new("pmr.aiki@gmail.com", emailcliente)
                {
                    Subject = subject,
                    IsBodyHtml = true, //si quieres incrustar en el body del tags html como links, imagenes,....
                    Body = cuerpoMensaje
                };

                if (!String.IsNullOrEmpty(ficheroAdjunto))
                {
                    //en el parametro fichero adjunto va la ruta y el nombre de la factura generada en el server...
                    MemoryStream ms = new();
                    using (FileStream fs = new(ficheroAdjunto, FileMode.Open, FileAccess.Read)) fs.CopyTo(ms);
                    _mensajeAEnviar.Attachments.Add(new Attachment(ms, "application/pdf"));
                }

                //3º: mandar email usando el socket abierto con cliente smtp creado <=== metodo .send de la clase
                //                                                                      SmtpClient
                _clienteSMTP.Send(_mensajeAEnviar);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        #endregion

    }
}

