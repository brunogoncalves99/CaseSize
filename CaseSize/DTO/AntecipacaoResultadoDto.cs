namespace CaseSize.DTO;

public class NotaFiscalResultadoDto
{
    public int Numero { get; set; }
    public decimal ValorBruto { get; set; }
    public decimal ValorLiquido { get; set; }
}

public class AntecipacaoResultadoDto
{
    public string Empresa { get; set; }
    public string CNPJ { get; set; }
    public decimal Limite { get; set; }
    public List<NotaFiscalResultadoDto> NotasFiscais { get; set; } = new List<NotaFiscalResultadoDto>();
    public decimal TotalLiquido { get; set; }
    public decimal TotalBruto { get; set; }
}
