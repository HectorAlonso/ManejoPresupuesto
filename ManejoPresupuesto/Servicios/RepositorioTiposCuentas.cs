using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioTiposCuentas
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int id);
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string Nombre, int UsuarioId);
        Task<IEnumerable<TipoCuenta>> Obtener(int UsuarioId);
        Task<TipoCuenta> ObtenerPorId(int Id, int UsuarioId);
        Task Ordenar(IEnumerable<TipoCuenta> TipoCuentaOrdenados);
    }

    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {
        private readonly string ConnectionString;
        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }


        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(ConnectionString);
            var id = await connection.QuerySingleAsync<int>("sp_TiposCuentas_Insertar", 
                                    new { UsuarioId = tipoCuenta.UsuarioId, Nombre = tipoCuenta.Nombre},
                                    commandType: System.Data.CommandType.StoredProcedure);

            //@"INSERT INTO TiposCuentas(Nombre, UsuarioId, Orden) VALUES(@Nombre, @UsuarioId, 0);SELECT SCOPE_IDENTITY()", tipoCuenta

            //El SCOPE_IDENTITY te permite regresar el Id del ultimo registro recien creado  
            tipoCuenta.Id = id;
        }


        public async Task<bool> Existe(string Nombre, int UsuarioId)
        {
            using var connection = new SqlConnection(ConnectionString);
            var Existe = await connection.QueryFirstOrDefaultAsync<int>(
                @"SELECT 1 FROM TiposCuentas
                WHERE Nombre = @Nombre and UsuarioId = @UsuarioId", new { Nombre, UsuarioId});

            return Existe == 1;
        }


        public async Task<IEnumerable<TipoCuenta>> Obtener(int UsuarioId)
        {
            using var connection = new SqlConnection(ConnectionString);
            return await connection.QueryAsync<TipoCuenta>(@"SELECT Id, Nombre,Orden FROM TiposCuentas 
                                                           WHERE UsuarioId = @UsuarioId
                                                           ORDER BY Orden", new { UsuarioId });
        }


        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(ConnectionString);
            await connection.ExecuteAsync("UPDATE TiposCuentas SET Nombre = @Nombre WHERE Id = @Id", tipoCuenta);
        }

        public async Task<TipoCuenta> ObtenerPorId(int Id, int UsuarioId)
        {
            using var connection = new SqlConnection(ConnectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden FROM TiposCuentas 
                                                           WHERE Id = @Id AND UsuarioId = @UsuarioId ", new { Id, UsuarioId });
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(ConnectionString);
            await connection.ExecuteAsync("DELETE FROM TiposCuentas WHERE Id = @Id", new { id });
        }

        public async Task Ordenar(IEnumerable<TipoCuenta> TipoCuentaOrdenados)
        {
            var query = "UPDATE TiposCuentas SET Orden = @Orden WHERE Id = @Id";
            using var connection = new SqlConnection(ConnectionString);
            await connection.ExecuteAsync(query, TipoCuentaOrdenados);
            //esta funcion permite cambiar el orden de cada registro por cada tipo de cuenta que se envie en el objeto TipoCuentaOrdenados
        }
    }
}
