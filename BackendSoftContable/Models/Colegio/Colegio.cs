using BackendSoftContable.Models;
using System;
using System.Collections.Generic;

namespace BackendSoftContable.Models
{
    public class Colegio: BaseEntity
    {
        public Guid Id { get; set; }

        public string NombreColegio { get; set; } = string.Empty;
        public string Nit { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;


        // 🔹 Relación con Ciudad 
        public int CiudadId { get; set; }
        public Ciudad Ciudad { get; set; } = null!;

        // 🔹 Relación con Régimen IVA
        public int RegimenIvaId { get; set; }
        public RegimenIva RegimenIva { get; set; } = null!;

        public string PlanSeleccionado { get; set; } = "Premium";
        public string? LogoPath { get; set; }
        public string? Telefono { get; set; }

        public string? TarifaIca { get; set; }

        public bool? ManejaAiu { get; set; }
        public bool? IvaRetencion { get; set; }
        public bool? UsaDobleImpuesto  { get; set; }
        public bool? usaImpuestoAdValorem { get; set; }

        // 🔹 Relación con Tributo
        public int TributoId { get; set; }
        public Tributo Tributo { get; set; } = null!;

        // 🔹 Relación con ResponsabilidadFiscal
        public int ResponsabilidadFiscalId { get; set; }
        public ResponsabilidadFiscal ResponsabilidadFiscal { get; set; } = null!;
        public int ActividadEconomicaId { get; set; }

        public ActividadEconomica ActividadEconomica { get; set; } = null!;


        // 🔹 Relación con REPRESENTANTES (Uno a Muchos)
        // Un colegio puede tener varios representantes legales
        public ICollection<RepresentanteLegal> RepresentantesLegales { get; set; } = new List<RepresentanteLegal>();

        // 🔹 Relación con usuarios
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}