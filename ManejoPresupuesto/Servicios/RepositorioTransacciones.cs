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
    public interface IRepositorioTransacciones
    {
        Task Actualizar(Transaccion transaccion, decimal MontoAnterior, int CuentaAnterior);
        Task Borrar(int id);
        Task Crear(Transaccion transaccion);
        Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo);
        Task<Transaccion> ObtenerPorId(int id, int usuarioId);
        Task<IEnumerable<ResultadoObtenerPorMes>> ObtenerPorMes(int usuarioId, int año);
        Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParametroObtenerTransaccionesPorUsuario modelo);
        Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParametroObtenerTransaccionesPorUsuario modelo);
    }

    public class RepositorioTransacciones : IRepositorioTransacciones
    {
        private readonly string connectionString;

        public RepositorioTransacciones(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Transaccion transaccion)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("Transacciones_Insertar",
            new
            {
                transaccion.UsuarioId,
                transaccion.FechaTransaccion,
                transaccion.Monto,
                transaccion.CategoriaId,
                transaccion.CuentaId,
                transaccion.Nota
            }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(@"
            SELECT T.Id, T.Monto, T.FechaTransaccion, C.Nombre As Categoria, CU.Nombre AS Cuenta, C.TipoOperacionId
            FROM Transacciones T
            INNER JOIN Categorias C
            ON C.Id = T.CategoriaId
            INNER JOIN Cuentas CU
            ON CU.Id = T.CuentaId
            WHERE T.CuentaId = @CuentaId AND T.UsuarioId = @usuarioId
            AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin", modelo);
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(@"
            SELECT T.Id, T.Monto, T.FechaTransaccion, C.Nombre As Categoria, CU.Nombre AS Cuenta, C.TipoOperacionId, Nota
            FROM Transacciones T
            INNER JOIN Categorias C
            ON C.Id = T.CategoriaId
            INNER JOIN Cuentas CU
            ON CU.Id = T.CuentaId
            WHERE T.UsuarioId = @usuarioId
            AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin
            ORDER BY T.FechaTransaccion DESC", modelo);
        }

        public async Task Actualizar(Transaccion transaccion, decimal MontoAnterior, int CuentaAnteriorId)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Actualizar", 
            new 
            { 
                transaccion.Id,
                transaccion.FechaTransaccion,
                transaccion.Monto,
                MontoAnterior,
                transaccion.CuentaId,
                CuentaAnteriorId,
                transaccion.CategoriaId,
                transaccion.Nota
            }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Transaccion> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(@"
            SELECT Transacciones.*, Cat.TipoOperacionId 
            FROM Transacciones 
            INNER JOIN Categorias Cat
            ON Transacciones.CategoriaId = Cat.Id
            WHERE Transacciones.Id = @Id AND Transacciones.UsuarioId = @UsuarioId;", new { id, usuarioId});
        }

        public async Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<ResultadoObtenerPorSemana>(@"
            SELECT datediff(d, @FechaInicio, FechaTransaccion) / 7 + 1 AS Semana, 
            SUM(Monto) AS Monto, CAT.TipoOperacionId
            FROM Transacciones
            INNER JOIN Categorias CAT
            ON CAT.Id = Transacciones.CategoriaId
            WHERE Transacciones.UsuarioId = @UsuarioId AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin 
            GROUP BY datediff(d, @FechaInicio, FechaTransaccion) / 7, CAT.TipoOperacionId", modelo);
        }

        public async Task<IEnumerable<ResultadoObtenerPorMes>> ObtenerPorMes(int usuarioId, int año)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<ResultadoObtenerPorMes>(@"
            SELECT MONTH(FechaTransaccion) as Mes, SUM(Monto) as Monto, Cat.TipoOperacionId
            FROM Transacciones
            INNER JOIN  Categorias Cat
            ON Cat.Id = Transacciones.CategoriaId
            WHERE Transacciones.UsuarioId = @usuarioId AND YEAR(FechaTransaccion) = @Año
            GROUP BY MONTH(FechaTransaccion), Cat.TipoOperacionId", new { usuarioId, año});
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"Transacciones_Borrar",
            new { id }, commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
