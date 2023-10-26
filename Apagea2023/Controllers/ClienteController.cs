using Apagea2023.Models;
using Apagea2023.Models.Servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Apagea2023.Controllers
{
    public class ClienteController : Controller
    {
        #region ... constructor ClienteController ...

        public ClienteController(IDBAccess servicioDBInyectado, IClienteCorreo servicioMailDI)
        {
            this.__servicioDB = servicioDBInyectado;
            this.__servicioMail = servicioMailDI;
        }

        #endregion

        #region ... propiedades ClienteController ...

        private IDBAccess __servicioDB;
        private IClienteCorreo __servicioMail;

        #endregion

        #region ... metodos ClienteController

        #region .. metodos de accion (generan vistas) ...

        #region ... metodos del REGISTRO ...
        [HttpGet]
        public IActionResult Registro()
        {
            return View(new Cliente()); //<--- metodo de la clase Controller q devuelve una vista, ¿donde?
                                        // dentro del directorio Views,-->  subdirectorio nombre_controlador, --> nombre_metodo.cshtml
        }

        [HttpPost]
        public IActionResult Registro(Cliente datoscliente,
                                      Boolean condUso,
                                      String repemail,
                                      String reppassword)
        {
            if (String.IsNullOrEmpty(datoscliente.Credenciales.Login))
            {
                datoscliente.Credenciales.Login = datoscliente.Credenciales.Email;
                if (ModelState.GetValidationState("Credenciales.Login") != ModelValidationState.Valid)
                {
                    ModelState.Remove("Credenciales.Login");
                }
            }

            if (repemail != datoscliente.Credenciales.Email || repemail == null)
            {
                //si en el 1º argumento del metodo .AddModelError pones "", el error de validacion no se asigna a ninguna prop.en especifico
                //si pones el nombre de una prop, p.e: Cliente.Credenciales.Email <--- el error de validacion apareceria asociado a prop.email del modelo Cliente.Crendenciales
                //y saldria en el span asociado al input email
                ModelState.AddModelError("", "* los Emails no coinciden");
            }

            if (reppassword != datoscliente.Credenciales.Password || reppassword == null)
            {
                ModelState.AddModelError("", "* las Password no coinciden");
            }

            if (!condUso)
            {
                ModelState.AddModelError("", "* Debes aceptar nuestras condiciones de uso");
            }

            if (ModelState.IsValid)
            {
                datoscliente.Credenciales.CuentaActiva = true;
                //1º insertar los datos en la bd
                bool _resultadoInsert = this.__servicioDB.RegistrarCliente(datoscliente);
                if (_resultadoInsert)
                {
                    //2º mandar email de activacion de cuenta
                    String __bodyEmail = @$"
                            <p>
                                Se te ha enviado un <strong>NUEVO CORREO a tu email: {datoscliente.Credenciales.Email} DE ACTIVACION</strong> de tu cuenta.
                            </p>
                            <p>
                                Pulsa en el siguiente <a href='https://localhost:7139/Cliente/ActivarCuenta/{datoscliente.IdCliente}'> ENLACE </a> para activar tu cuenta
                            </p>
                    ";
                    _ = this.__servicioMail.EnviarEmail(datoscliente.Credenciales.Email, "Activa tu cuenta en Agapea.com", __bodyEmail, "");

                    //3º redirigir a vista de registro ok
                    return View("RegistroOK"); //return RedirectToAction("RegistroOK")
                }
                else
                {
                    //mostrar en vista mensaje de error: "Ha habido un fallo interno, intentelo de nuevo mas tarde..."
                    //mostrar vista de Registro.cshtml con ese error
                    return View("Registro");
                }
            }
            else
            {
                //fallos de validacion, devuelvo la vista Registro.cshtml con el objeto modelo Cliente con los errores
                return View(datoscliente);
            }
        }

        [HttpGet]
        public IActionResult ActivarCuenta(String id)
        {
            //recibo como parametro en el metodo de accion, el tercer segmento de la url, q es el idCliente del q quiero activar la cuenta
            // usando el servicio de acceso a datos, modificar la tabla Clientes, el registro de ese idCliente y poner columna CuentaActiva a true
            if (this.__servicioDB.ActivarCuentaCliente(id))
            {
                return RedirectToAction("Login");
            }
            else
            {
                //el fallo puede ser debido a:
                // - o bien q el idcliente q pasan en la url no exista....habria q comprobar si existe ese id en la tabla clientes
                //     si existe, vuelves a mandar el email de activacion, sino existe rediriges al registro

                // - o bien fallo en el update por caida en el servidor de bd..reintentar el activar cuenta de nuevo  o bien mandas
                //   un nuevo mail de activacion....
                return RedirectToAction("Registro");
            }
        }

        #endregion

        #region ... metodos del LOGIN ...

        [HttpGet]
        public IActionResult Login()
        {
            return View(new Cuenta());
        }

        [HttpPost]
        public IActionResult Login(Cuenta datoslogin)
        {
            //no pregunto por el estado de validacion del modelo completo, solo me interesan dos propiedades q son los q estan en el
            //formulario, el EMAIL y la PASSWORD...son los q me interesan q pasen los dataannotations, el resto me da igual:
            if (ModelState.GetFieldValidationState("Email") == ModelValidationState.Valid &&
               ModelState.GetFieldValidationState("Password") == ModelValidationState.Valid)
            {
                //1º compruebo q la cuenta esta activada...sino esta activada, vuelvo a mandar email de activacion

                //2º si ok, compruebo credenciales...
                Cliente _datosCliente = this.__servicioDB.LoginCliente(datoslogin.Email, datoslogin.Password);
                if (_datosCliente == null)
                {
                    ModelState.AddModelError("", "* Email o Password incorrectas, intentelo de nuevo");
                    return View(datoslogin);
                }
                else
                {
                    //3º si ok, CREO VARIABLE DE ESTADO con los datos del objeto cliente logueado (pedidos, direcciones, opiniones,...)
                    HttpContext.Session.SetString("datoscliente", JsonSerializer.Serialize<Cliente>(_datosCliente));
                    HttpContext.Session.SetString("pedidoactual", JsonSerializer.Serialize<Pedido>(new Pedido()));
                    return RedirectToAction("MostrarLibros", "Tienda");

                }
            }
            else
            {
                return View(datoslogin);
            }
        }

        #endregion

        #endregion

        #region ... metodos privados de la clase

        #endregion
        #endregion
    }
}

