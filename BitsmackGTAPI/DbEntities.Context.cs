﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BitsmackGTAPI
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class BSGTEntities : DbContext
    {
        public BSGTEntities()
            : base("name=BSGTEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Cardio> Cardio { get; set; }
        public DbSet<Pedometer> Pedometer { get; set; }
        public DbSet<APIKeys> APIKeys { get; set; }
        public DbSet<EventLog> EventLog { get; set; }
    }
}
