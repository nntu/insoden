//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace insoden
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class bdsuEntities : DbContext
    {
        public bdsuEntities()
            : base("name=bdsuEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tbPrinter> tbPrinters { get; set; }
        public virtual DbSet<tbsothe> tbsothes { get; set; }
        public virtual DbSet<tbbdsuser> tbbdsusers { get; set; }
        public virtual DbSet<tbXoaATM> tbXoaATMs { get; set; }
        public virtual DbSet<tb_ql_CIF> tb_ql_CIF { get; set; }
        public virtual DbSet<tblBranch> tblBranches { get; set; }
        public virtual DbSet<dept> depts { get; set; }
    }
}
