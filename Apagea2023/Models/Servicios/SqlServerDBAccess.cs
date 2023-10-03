using System.Data.SqlClient;
using BCrypt.Net;
using Apagea2023.Models.Servicios.Interfaces;

namespace Apagea2023.Models.Servicios
{
	public class SqlServerDBAccess : IDBAccess
    {
        #region ... constructor modelo servicio SQL Server ...

        public SqlServerDBAccess(IConfiguration servicioAccesoAppSettingsDI)
        {
            this.__accesoAppSettings = servicioAccesoAppSettingsDI;
        }

        #endregion

        #region ... propiedades clase servicio sql server ...

        private IConfiguration __accesoAppSettings;
        private String __cadenaConexionServer;

        #endregion

        #region ... metodos clase servicio SQL Server ...

        #region ... metodos Cliente Controller ...

        public Cliente? LoginCliente(string email, string password)
        {
            try
            {
                using SqlConnection _conexionBD = new(this.__cadenaConexionServer);
                Cliente _clientaADevolver = new();

                _conexionBD.Open();

                //1º recupero datos sobre tabla Clientes....
                SqlCommand _selectCliente = new ("SELECT * FROM dbo.Clientes WHERE Email=@em", _conexionBD);
                _selectCliente.Parameters.AddWithValue("@em", email);

                SqlDataReader _cursor = _selectCliente.ExecuteReader();
                if (_cursor.HasRows)
                {
                    //leo el cursor...
                    while (_cursor.Read())
                    {
                        //compruebo si los hashes coinciden...
                        if (BCrypt.Net.BCrypt.Verify(password, _cursor["Password"].ToString()))
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
                using SqlConnection _conexionBD = new (this.__cadenaConexionServer);
                _conexionBD.Open();

                SqlCommand _updateCliente = new ("UPDATE dbo.Clientes SET CuentaActiva=1 WHERE IdCliente=@id", _conexionBD);
                _updateCliente.Parameters.AddWithValue("@id", idCliente);

                int _resultadoUpdate = _updateCliente.ExecuteNonQuery();
                return _resultadoUpdate == 1;
            }
            catch (Exception )
            {
                return false;
            }
        }

        #endregion

        #endregion
    }
}

