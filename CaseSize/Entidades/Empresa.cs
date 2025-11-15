using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CaseSize.Entidades;

namespace CaseSize.Entitades
{
    public class Empresa
    {
        [Key]
        public int EmpresaId { get; set; }
        [Required]
        [StringLength(14)]
        public string CNPJ { get; set; }

        [Required]
        [StringLength(255)]
        public string Nome { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal FaturamentoMensal { get; set; }
        public RamoEmpresa Ramo { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal LimiteCredito { get; set; }

        // Propriedade de navegação
        public ICollection<NotaFiscal> NotasFiscais { get; set; } = new List<NotaFiscal>();
    }
}
