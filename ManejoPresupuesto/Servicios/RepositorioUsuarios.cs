using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioUsuarios
    {
        Task<Usuario> BuscarUsuarioPorEmail(string emailNormalizado);
        Task<int> CrearUsuario(Usuario usuario);
    }

    public class RepositorioUsuarios: IRepositorioUsuarios
    {
        private readonly string connectionString;

        public RepositorioUsuarios(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CrearUsuario(Usuario usuario)
        {
            using var connection = new SqlConnection(connectionString);
            var usuarioId = await connection.QuerySingleAsync<int>(@"
            INSERT INTO Usuarios (Email, EmailNormalizado, PasswordHash)
            VALUES(@Email, @EmailNormalizado, @PasswordHash);
            SELECT SCOPE_IDENTITY();", usuario);

            //Crear datos por defecto al registrar un nuevo usuario
            await connection.ExecuteAsync("CrearDatosUsuarioNuevo", new { usuarioId }, 
                commandType: System.Data.CommandType.StoredProcedure);

            return usuarioId;
        }

        public async Task<Usuario> BuscarUsuarioPorEmail(string emailNormalizado)
        {
            using var connection = new SqlConnection(connectionString);
            var usuario = await connection.QueryFirstOrDefaultAsync<Usuario>(@"
            SELECT * FROM Usuarios WHERE EmailNormalizado = @EmailNormalizado", new { emailNormalizado });

            return usuario;
        }
    }
}
