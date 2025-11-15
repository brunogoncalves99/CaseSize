using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaseSize.Entitades;

public class NotaFiscal
{
    [Key]
    public int NotaFiscalId { get; set; }
    [Required]
    public int Numero { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal ValorBruto { get; set; }

    [Required]
    public DateTime DataVencimento { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Pendente"; // Pendente, Antecipada

    public int EmpresaId { get; set; }

    // Propriedade de navegação com a chave estrangeira EmpresaId
    [ForeignKey("EmpresaId")]
    public Empresa? Empresa { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal ValorLiquido { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Desagio { get; set; }
}
