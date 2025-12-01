using CaseSize.DTO;
using CaseSize.Service;

namespace CaseSize.Interface
{
    public interface IAntecipacao
    {
        Task<AntecipacaoResultadoDto> ProcessamentoAntecipacao(int idEmpresa, List<int> notasFiscaisIds);
        (decimal ValorLiquido, decimal Desagio) CalcularAntecipacao(decimal valorBruto, DateTime dataVencimento, DateTime dataAtual);
    }
}
