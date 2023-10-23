using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apagea2023.Models;
using Apagea2023.Models.Servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Apagea2023.Controllers
{
    public class TiendaController : Controller
    {
        #region ... propiedades controlador Tienda ...
        private IDBAccess __servicioBD;

        #endregion

        public TiendaController(IDBAccess servicioDBInyectado)
        {
            this.__servicioBD = servicioDBInyectado;
        }





        #region ... metodos controlador Tienda ...

        #region ... METODOS DE ACCION ....
        [HttpGet]
        public IActionResult MostrarLibros(String id)
        {
            // accion del controlador para mostrar libros de una determinada categoria... la categoria iria por el
            // 3 segmento de la url mapeada contra el parametro id
            // si el parametro id es vacio, se recuperarian las OFERTAS DEL MES, o las NOVEDADES, ...
            // si el id esta vacio muestro los libros de las categorias informaticas 2-10
            if (String.IsNullOrEmpty(id))   id = "2-10";

            List<Libro> _listaLibrosCat = this.__servicioBD.RecuperarLibros(id) ?? new List<Libro>();

            return View(_listaLibrosCat);
        }

        [HttpGet]
        public IActionResult MostrarDetallesLibro(String id,
                                                  [FromQuery] String titulo,
                                                  [FromQuery] String ISBN10)
        {
            Libro _libroAPintar = this.__servicioBD.DevuelveLibro(id);
            return View(_libroAPintar);
        }

        #endregion

        #region ............. METODOS privados ..........

        #endregion

        #endregion

    }
}