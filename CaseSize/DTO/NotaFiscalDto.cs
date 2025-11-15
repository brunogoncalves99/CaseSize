using System.ComponentModel.DataAnnotations;

namespace CaseSize.DTO;
public class NotaFiscalDto
{
    [Required(ErrorMessage = "O ID da Empresa é obrigatório.")]
    public int EmpresaId { get; set; }

    [Required(ErrorMessage = "O Número da Nota Fiscal é obrigatório.")]
    public int Numero { get; set; }

    [Required(ErrorMessage = "O Valor é obrigatório.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O Valor deve ser positivo.")]
    public decimal ValorBruto { get; set; }

    [Required(ErrorMessage = "A Data de Vencimento é obrigatória.")]
    public DateTime DataVencimento { get; set; }


}
