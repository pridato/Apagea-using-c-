﻿using System;
namespace Apagea2023.Models.Servicios.Interfaces
{
	public interface IDBAccess
	{

        #region ... propiedades interface IDBAccess ...

        #endregion

        #region ... metodos interface IDBAccess ...

        #endregion

        #region ... metodos para ClienteController ...

        public Boolean RegistrarCliente(Cliente datoscliente);
        public Cliente LoginCliente(String email, String password);
        public Boolean ActivarCuentaCliente(String idCliente);

        #endregion

        #region ... metodos para TiendaController ...

        public List<Libro>? RecuperarLibros(String idcategoria);
        public List<Categoria> RecuperaCategorias(String idcategoria);
        public Libro DevuelveLibro(string id);
        #endregion
    }
}

