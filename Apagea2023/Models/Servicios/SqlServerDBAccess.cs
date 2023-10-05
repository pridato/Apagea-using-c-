using System.Data.SqlClient;
using BCrypt.Net;
using Apagea2023.Models.Servicios.Interfaces;
using System.Data;

namespace Apagea2023.Models.Servicios
{
    public class SqlServerDBAccess : IDBAccess
    {
        #region ... constructor modelo servicio SQL Server ...

        public SqlServerDBAccess(IConfiguration servicioAccesoAppSettingsDI)
        {
            this.__accesoAppSettings = servicioAccesoAppSettingsDI;
            this.__cadenaConexionServer = this.__accesoAppSettings
                                               .GetSection("ConnectionStrings:SqlServerConnection")
                                               .Value;
        }

        #endregion

        #region ... propiedades clase servicio sql server ...

        private IConfiguration __accesoAppSettings;
        private String __cadenaConexionServer;

        #endregion

        #region ... metodos clase servicio SQL Server ...

        #region ... metodos Cliente Controller ...

        public Cliente LoginCliente(string email, string password)
        {
            try
            {
                using SqlConnection _conexionBD = new(this.__cadenaConexionServer);
                Cliente _clientaADevolver = new();

                _conexionBD.Open();

                //1º recupero datos sobre tabla Clientes....
                SqlCommand _selectCliente = new("SELECT * FROM dbo.Clientes WHERE Email=@em", _conexionBD);
                _selectCliente.Parameters.AddWithValue("@em", email);

                SqlDataReader _cursor = _selectCliente.ExecuteReader();
                if (_cursor.HasRows)
                {
                    //leo el cursor...
                    while (_cursor.Read())
                    {
                        //compruebo si los hashes coinciden...
                        if (BCrypt.Net.BCrypt.Verify(password, _cursor["PasswordHash"].ToString()))
                        {
                            _clientaADevolver.IdCliente = _cursor["IdCliente"].ToString() ?? "sin IdCliente";
                            _clientaADevolver.Nombre = _cursor["Nombre"].ToString() ?? "sin nombre";
                            _clientaADevolver.Apellidos = _cursor["Apellidos"].ToString() ?? "sin apellidos";
                            _clientaADevolver.Telefono = _cursor["Telefono"].ToString() ?? "sin tlfno";
                            _clientaADevolver.Credenciales.Email = _cursor["Email"].ToString() ?? "sin email";
                            _clientaADevolver.Credenciales.Login = _cursor["Login"].ToString() ?? "sin login";
                            _clientaADevolver.Credenciales.Password = "";
                            _clientaADevolver.Credenciales.CuentaActiva = System.Convert.ToBoolean(_cursor["CuentaActiva"]);
                            _clientaADevolver.Credenciales.ImagenCuentaBASE64 = _cursor["ImagenCuentaBASE64"].ToString() ?? "";
                        }
                        else
                        {
                            throw new Exception("password incorrecta");
                        }
                    }
                }
                else
                {
                    throw new Exception("ese email no existe");
                }

                //2º recupero datos tabla Direcciones y las añado al objeto Cliente a devolver...
                //3º recupero datos tabla Pedidos y las añado al objeto Cliente a devolver...
                //4º recupero datos tabla Opiniones y las añado al objeto Cliente a devolver...


                return _clientaADevolver;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public bool RegistrarCliente(Cliente datoscliente)
        {
            SqlConnection _conexionBD = null;
            try
            {
                this.__cadenaConexionServer = this.__accesoAppSettings
                    .GetSection("ConnectionStrings:SqlServerConnection")
                    .Value;
                //1º paso: abrir la conexion a la bd en el servidor sql-server usando cadena conexion
                _conexionBD = new SqlConnection(this.__cadenaConexionServer);
                _conexionBD.Open();

                //2º paso: definir el INSERT y ejecutarlo en tabla Clientes de la bd Agapea2024
                SqlCommand _insertClientes = new()
                {
                    Connection = _conexionBD,
                    CommandText = "INSERT INTO dbo.Clientes (" +
                                   "IdCliente, Nombre, Apellidos, Telefono, Email, PasswordHash, Login, CuentaActiva, ImagenCuentaBASE64)\n" +
                                   "VALUES (@id, @nom, @ape, @tlfno, @email, @paswd, @log, @act, @img)\n",
                };
                _insertClientes.Parameters.AddWithValue("@id", datoscliente.IdCliente);
                _insertClientes.Parameters.AddWithValue("@nom", datoscliente.Nombre);
                _insertClientes.Parameters.AddWithValue("@ape", datoscliente.Apellidos);
                _insertClientes.Parameters.AddWithValue("@tlfno", datoscliente.Telefono);
                _insertClientes.Parameters.AddWithValue("@email", datoscliente.Credenciales.Email);

                //....hay q sacar el hash de la password para insertarlo en la tabla....
                String __hashPassword = BCrypt.Net.BCrypt.HashPassword(datoscliente.Credenciales.Password);
                _insertClientes.Parameters.AddWithValue("@paswd", __hashPassword);

                _insertClientes.Parameters.AddWithValue("@log", datoscliente.Credenciales.Login);
                _insertClientes.Parameters.AddWithValue("@act", datoscliente.Credenciales.CuentaActiva);
                _insertClientes.Parameters.AddWithValue("@img", datoscliente.Credenciales.ImagenCuentaBASE64);


                //3º paso, si todo ok devolver true si fallo devolver false
                int _numfilasInsertadas = _insertClientes.ExecuteNonQuery();
                return _numfilasInsertadas == 1;

            }
            catch (Exception)
            {
                return false;

            }
            finally
            {
                _conexionBD?.Close();
            }

        }

        public bool ActivarCuentaCliente(string idCliente)
        {
            try
            {
                using SqlConnection _conexionBD = new(this.__cadenaConexionServer);
                _conexionBD.Open();

                SqlCommand _updateCliente = new("UPDATE dbo.Clientes SET CuentaActiva=1 WHERE IdCliente=@id", _conexionBD);
                _updateCliente.Parameters.AddWithValue("@id", idCliente);

                int _resultadoUpdate = _updateCliente.ExecuteNonQuery();
                return _resultadoUpdate == 1;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion


        #region ... metodos Tienda controller ...

        public List<Categoria> RecuperaCategorias(string idCategorias)
        {
            // si en parametro idCategoria viene valor "raiz" <----- invocado desde _layout para el panel lateral
            // sino vendra un idCategoria del que quiero saber sus subcategorias: ej 2-10 informatica <- subcats. Bd.Programacion
            if (idCategorias.Equals("root"))
            {
                try
                {
                    using SqlConnection _conexionBD = new(this.__cadenaConexionServer);
                    _conexionBD.Open();

                    SqlCommand _selectCategorias = new("Select * From dbo.Categorias", _conexionBD);

                    return _selectCategorias.ExecuteReader()
                                            .Cast<IDataRecord>()
                                            .Select((IDataRecord fila) => new Categoria
                                            {
                                                IdCategoria = fila["IdCategoria"].ToString() ?? "",
                                                NombreCategoria = fila["NombreCategoria"].ToString() ?? "",
                                            })
                                            .ToList<Categoria>();
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return null;

        }

        public List<Libro>? RecuperarLibros(string idcategoria)
        {

            try
            {
                using SqlConnection _conexionBD = new SqlConnection(this.__cadenaConexionServer);
                _conexionBD.Open();
                SqlCommand _selectLibros = new SqlCommand("Select * From dbo.Libros WHERE IdCategoria LIKE @idcat + '%'", _conexionBD);
                _selectLibros.Parameters.AddWithValue("@idcat", idcategoria.ToString());

                /* todo esto forma original 
                if (_resSelect.HasRows)
                {
                    List<Libro> _listaADevolver = new List<Libro>();
                    while (_resSelect.Read())
                    {
                        Libro _libro = new();
                        _libro.IdCategoria = _resSelect["IdCategoria"].ToString() ?? "";
                        _libro.Titulo = _resSelect["Titulo"].ToString() ?? "";
                        _libro.Autores = _resSelect["Autores"].ToString() ?? "";
                        _libro.ISBN10 = _resSelect["ISBN10"].ToString() ?? "";
                        _libro.ISBN13 = _resSelect["ISBN13"].ToString() ?? "";
                        _libro.Edicion = _resSelect["Edicion"].ToString() ?? "";
                        _libro.Dimensiones = _resSelect["Dimensiones"].ToString() ?? "";
                        _libro.ImagenLibroBASE64 = _resSelect["ImagenLibroBASE64"].ToString() ?? "";
                        _libro.Idioma = _resSelect["Idioma"].ToString() ?? "";
                        _libro.Resumen = _resSelect["Resumen"].ToString() ?? "";
                        _libro.NumeroPaginas = System.Convert.ToInt32(_resSelect["NumeroPaginas"].ToString() ?? "");
                        _libro.Precio = System.Convert.ToInt32(_resSelect["IdCategoria"].ToString() ?? "");

                        _listaADevolver.Add(_libro);
                    }
                    return _listaADevolver;
                else {
                ... } exception...
                */
                return _selectLibros.ExecuteReader()
                             .Cast<IDataRecord>()
                             .Select((IDataRecord fila) => new Libro
                             {
                                 IdCategoria = fila["IdCategoria"].ToString() ?? "",
                                 Titulo = fila["Titulo"].ToString() ?? "",
                                 Autores = fila["Autores"].ToString() ?? "",
                                 ISBN10 = fila["ISBN10"].ToString() ?? "",
                                 ISBN13 = fila["ISBN13"].ToString() ?? "",
                                 Edicion = fila["Edicion"].ToString() ?? "",
                                 Dimensiones = fila["Dimensiones"].ToString() ?? "",
                                 ImagenLibroBASE64 = fila["ImagenLibroBASE64"].ToString() ?? "",
                                 Idioma = fila["Idioma"].ToString() ?? "",
                                 Resumen = fila["Resumen"].ToString() ?? "",
                                 NumeroPaginas = System.Convert.ToInt32(fila["NumeroPaginas"].ToString() ?? ""),
                                 Precio = System.Convert.ToDecimal(fila["Precio"].ToString() ?? ""),
                             })
                             .ToList<Libro>();
                
            }
            catch (Exception)
            {
                return null;
            } 
        }

        #endregion
        #endregion
    }
}

