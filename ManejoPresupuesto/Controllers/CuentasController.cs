﻿using AutoMapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManejoPresupuesto.Controllers
{
    public class CuentasController : Controller
    {
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IServiciosUsuarios serviciosUsuarios;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IMapper mapper;
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IServicioReportes servicioReportes;

        public CuentasController(IRepositorioTiposCuentas repositorioTiposCuentas,
            IServiciosUsuarios serviciosUsuarios, IRepositorioCuentas repositorioCuentas,
            IMapper mapper, IRepositorioTransacciones repositorioTransacciones,
            IServicioReportes servicioReportes)
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.serviciosUsuarios = serviciosUsuarios;
            this.repositorioCuentas = repositorioCuentas;
            this.mapper = mapper;
            this.repositorioTransacciones = repositorioTransacciones;
            this.servicioReportes = servicioReportes;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = serviciosUsuarios.ObtenerUsuarioId();
            var cuentasConTipoCuenta = await repositorioCuentas.Buscar(usuarioId);

            var modelo = cuentasConTipoCuenta
                .GroupBy(x => x.TipoCuenta)
                .Select(grupo => new IndiceCuentasViewModel
                {
                    TipoCuenta = grupo.Key,
                    Cuentas = grupo.AsEnumerable()
                }).ToList();

            return View(modelo);
        }

        public async Task<IActionResult> Detalle(int id, int mes, int año)
        {
            var usuarioId = serviciosUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            ViewBag.Cuenta = cuenta.Nombre;

            var modelo = await servicioReportes
                .ObtenerReporteTransaccionesDetalladasPorCuenta(usuarioId, id, mes, año, ViewBag);

            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var usuarioId = serviciosUsuarios.ObtenerUsuarioId();
            var modelo = new CuentaCreacionViewModel();
            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CuentaCreacionViewModel cuenta)
        {
            var usuarioId = serviciosUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(cuenta.TipoCuentaId, usuarioId);

            if(tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            if (!ModelState.IsValid)
            {
                cuenta.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
                return View(cuenta);
            }

            await repositorioCuentas.Crear(cuenta);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = serviciosUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var modelo = mapper.Map<CuentaCreacionViewModel>(cuenta);

            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(CuentaCreacionViewModel cuentaE)
        {
            var usuarioId = serviciosUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(cuentaE.Id, usuarioId);

            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(cuentaE.TipoCuentaId, usuarioId);

            if(tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioCuentas.Actualizar(cuentaE);
            return RedirectToAction("Index");
        }

        

        [HttpGet]
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = serviciosUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, usuarioId);

            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(cuenta);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarCuenta(int id)
        {
            var usuarioId = serviciosUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioCuentas.Borrar(id);

            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId)
        {
            var tiposCuentasDelUsuario = await repositorioTiposCuentas.Obtener(usuarioId);
            return tiposCuentasDelUsuario.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }
    }
}