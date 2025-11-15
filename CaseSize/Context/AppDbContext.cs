using CaseSize.Entitades;
using Microsoft.EntityFrameworkCore;


namespace CaseSize.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
    }

    public DbSet<Empresa> Empresas { get; set; } // Tabela de Empresas
    public DbSet<NotaFiscal> NotasFiscais { get; set; } // Tabela de Notas Fiscais

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuração da entidade Empresa
        modelBuilder.Entity<Empresa>()
            .HasIndex(e => e.CNPJ)
            .IsUnique();

        // Mapeamento do Enum RamoEmpresa para string no banco de dados
        modelBuilder.Entity<Empresa>()
            .Property(e => e.Ramo)
            .HasConversion<string>();

        // Configuração da entidade NotaFiscal
        modelBuilder.Entity<NotaFiscal>()
            .Property(nf => nf.Status)
            .HasConversion<string>();

        // Configuração de chave estrangeira
        modelBuilder.Entity<NotaFiscal>()
            .HasOne(nf => nf.Empresa)
            .WithMany(e => e.NotasFiscais)
            .HasForeignKey(nf => nf.EmpresaId);

        base.OnModelCreating(modelBuilder);
    }
}
