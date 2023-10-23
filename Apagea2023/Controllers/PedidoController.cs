using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Apagea2023.Controllers
{
    public class PedidoController : Controller
    {

        #region ... propiedades del controlador Pedido ...

        #endregion

        #region ... metodos de clase controlador Pedido ...

        #region metodos de accion

        [HttpGet]
        public IActionResult MostrarPedido()
        {
            // 1º recuperar el estado de sesion del cliente que se conexta al server, para recuperar datos del cliente logueao
            return View();
        }

        #endregion

        #region metodos privados

        #endregion
        #endregion

    }
}