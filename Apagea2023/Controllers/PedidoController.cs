using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Apagea2023.Models;
using Apagea2023.Models.Servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Apagea2023.Controllers
{
    public class PedidoController : Controller
    {

        #region ... propiedades del controlador Pedido ...
        private IDBAccess _servicioDB;
        #endregion

        public PedidoController(IDBAccess servicioDBInyectado)
        {
            this._servicioDB = servicioDBInyectado;
        }

        #region ... metodos de clase controlador Pedido ...

        #region metodos de accion

        [HttpGet]
        public async Task<IActionResult> MostrarPedido()
        {
            // 1º recuperar el estado de sesion del cliente que se conexta al server, para recuperar datos del cliente logueao
            //Recuperar el pedido actual siempre en try catch x si el token se ha desaktivao y es null
            try
            {
                Cliente? _datoscliente = JsonSerializer.Deserialize<Cliente>(HttpContext.Session.GetString("datoscliente") ?? "");
                Pedido? _pedidoactual = JsonSerializer.Deserialize<Pedido>(HttpContext.Session.GetString("pedidoactual") ?? "");

                // 2º tengo k pasar la lista de provincias recuperadas del servicio externo a la vista
                //3 formas:
                // -- ViewData <-- diccionario clave-valor masusao
                // -- ViewBag  <-- objeto dinamico como props () y tienes k estar continuamente Casting
                // --TempData  <-- diccionario clave- valor s mantiene entre dos, s diferencia del ViewData k mantiene su valor entre 2 peticiones (lo puedes defnir en un metodo de accion y recuperarlo en otro)

                List<Provincia> _provincias = new();
                HttpClient _httpClient = new();
                APIRestMessage? _respuestaREST = await _httpClient.GetFromJsonAsync<APIRestMessage>("https://apiv1.geoapi.es/provincias?type=JSON&key=&sandbox=1");

                _provincias = _respuestaREST.Data
                                            .Select((JsonElement item) => JsonSerializer.Deserialize<Provincia>(item))
                                            .ToList<Provincia>();

                ViewData["provincias"] = _provincias;
                ViewData["datoscliente"] = _datoscliente;
                ViewData["pedidoactual"] = _pedidoactual;

                return View(_pedidoactual);
            }
            catch (Exception error)
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult AddLibroPedido(String id)
        {
            // en el parametro id va el isbn13 del libro a añadir al pedido actual
            // tengo que comprobar si existe o no, en caso afirmativo incremento la cant. en uno,
            // sino añado nuevo elem
            try
            {
                Pedido? _pedidoactual = JsonSerializer.Deserialize<Pedido>(HttpContext.Session.GetString("pedidoactual") ?? "");
                int _positionLibro = _pedidoactual.ElementosPedido.FindIndex((ItemPedido item) => item.LibroItem.ISBN13 == id);

                if (_positionLibro == -1)
                {
                    Libro _libroRecup = this._servicioDB.DevuelveLibro(id);

                    _pedidoactual.ElementosPedido.Add(new ItemPedido { LibroItem = _libroRecup, CantidadItem = 1 });
                }
                else
                {
                    _pedidoactual.ElementosPedido[_positionLibro].CantidadItem += 1;
                }

                HttpContext.Session.SetString("pedidoactual", JsonSerializer.Serialize<Pedido>(_pedidoactual));
                return RedirectToAction("MostrarPedido");
            }
            catch (Exception err)
            {
                return RedirectToAction("MostrarPedido");
            }
        }
        #endregion

        [HttpGet]
        public IActionResult SumarCantidadLibroPedido(String isbn13)
        {
            Pedido? _pedidoactual = JsonSerializer.Deserialize<Pedido>(HttpContext.Session.GetString("pedidoactual") ?? "");
            int posicion = _pedidoactual?.ElementosPedido.FindIndex((ItemPedido item) => item.LibroItem.ISBN13 == isbn13) ?? -1;
            _pedidoactual.ElementosPedido[posicion].CantidadItem += 1;
            return RedirectToAction("MostrarPedido");
        }

        [HttpGet]
        public IActionResult OperarLibroPedido(String id, [FromQuery] String operacion)
        {

            try
            {
                Pedido? _pedidoactual = JsonSerializer.Deserialize<Pedido>(HttpContext.Session.GetString("pedidoactual") ?? "");
                int posicion = _pedidoactual?.ElementosPedido.FindIndex((ItemPedido item) => item.LibroItem.ISBN13 == id) ?? -1;

                if (posicion != -1)
                {
                    switch (operacion)
                    {
                        case "eliminar":
                            _pedidoactual?.ElementosPedido.RemoveAt(posicion);
                            break;
                        case "sumar":
                            if (_pedidoactual != null)
                                _pedidoactual.ElementosPedido[posicion].CantidadItem += 1;
                            break;
                        case "restar":
                            if (_pedidoactual != null)
                            {
                                if (_pedidoactual.ElementosPedido[posicion].CantidadItem >= 1)
                                {
                                    _pedidoactual.ElementosPedido[posicion].CantidadItem -= 1;
                                }
                                else
                                {
                                    _pedidoactual.ElementosPedido.RemoveAt(posicion);
                                }
                            }
                            break;
                    }

                    HttpContext.Session.SetString("pedidoactual", JsonSerializer.Serialize<Pedido>(_pedidoactual));
                    return RedirectToAction("MostrarPedido");

                }
                else
                {
                    ViewData["errores"] = "ISBN DEL LIBRO NO EXISTE";

                }
                return _pedidoactual?.ElementosPedido.Count != 0 ? RedirectToAction("MostrarPedido") : RedirectToAction("MostrarLibros", "Tienda");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Cliente");
            }


        }

        [HttpPost]
        public IActionResult FinalizarPedido()
        {
            // 1º almacenar pedido en BD ( si hay direcciones nuevas de envio y/o facturacion,
            // almacenarlas en BD



            // 2º generacion factura en pdf y envio de email al cliente con factura



            // 3º redirigir al panel del cliente, a "MisPedidos"



            return View();
        }

        #region metodos privados

        #endregion
        #endregion

    }
}