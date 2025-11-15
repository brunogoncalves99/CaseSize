using CaseSize.Entidades;
using System.ComponentModel.DataAnnotations;

namespace CaseSize.DTO;

public class EmpresaDto
{
    [Required(ErrorMessage = "O CNPJ é obrigatório.")]
    [StringLength(14, MinimumLength = 14, ErrorMessage = "O CNPJ deve ter 14 caracteres.")]
    public string CNPJ { get; set; }

    [Required(ErrorMessage = "O Nome é obrigatório.")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O Faturamento Mensal é obrigatório.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O Faturamento Mensal deve ser positivo.")]
    public decimal FaturamentoMensal { get; set; }

    [Required(ErrorMessage = "O Ramo é obrigatório.")]
    public RamoEmpresa Ramo { get; set; }

}