﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAOicom
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class icomEntities : DbContext
    {
        public icomEntities()
            : base("name=icomEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<componentes> componentes { get; set; }
        public virtual DbSet<equipoauxiliar> equipoauxiliar { get; set; }
        public virtual DbSet<filtros> filtros { get; set; }
        public virtual DbSet<maquinas> maquinas { get; set; }
        public virtual DbSet<mediciones> mediciones { get; set; }
        public virtual DbSet<medicionesfiltros> medicionesfiltros { get; set; }
        public virtual DbSet<puestos> puestos { get; set; }
        public virtual DbSet<refacciones_reporte> refacciones_reporte { get; set; }
        public virtual DbSet<reportes> reportes { get; set; }
        public virtual DbSet<tipofallas> tipofallas { get; set; }
        public virtual DbSet<tipomantenimientos> tipomantenimientos { get; set; }
        public virtual DbSet<usuarios> usuarios { get; set; }
        public virtual DbSet<tipomaquina> tipomaquina { get; set; }
        public virtual DbSet<statusreporte> statusreporte { get; set; }
    }
}
