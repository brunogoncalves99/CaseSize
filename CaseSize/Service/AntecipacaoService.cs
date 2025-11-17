using CaseSize.Context;
using CaseSize.DTO;
using CaseSize.Entidades;
using CaseSize.Entitades;
using Microsoft.EntityFrameworkCore;

namespace CaseSize.Service;
public class AntecipacaoService
{

    private readonly AppDbContext _dbContext;
    private const decimal TaxaMes = 0.0465m; // Taxa de desconto mensal de 4,65%

    public AntecipacaoService(AppDbContext dbContext)
    {
        _dbContext = dbContext; // Injeção de dependência do DbContext
    }

    #region Processamento de Antecipação
    public async Task<AntecipacaoResultadoDto> ProcessamentoAntecipacao(int idEmpresa, List<int> notasFiscaisIds)
    {
        decimal totalLiquido = 0;
        DateTime dataAtual = DateTime.Today;

        var empresa = await _dbContext.Empresas.FindAsync(idEmpresa); 
        if (empresa == null)
        {
            throw new KeyNotFoundException(Resources.Resources.EmpresaNaoEncontrada);
        }
        // Validação das Notas Fiscais // Ignorar antecipadas // Seleciona apenas as notas pedidas
        var notasFiscais = await _dbContext.NotasFiscais.Where(nf=> notasFiscaisIds.Contains(nf.NotaFiscalId) && nf.EmpresaId == idEmpresa && nf.Status != "Antecipada").ToListAsync();

        if (notasFiscais.Count != notasFiscaisIds.Count)
        {
            throw new InvalidOperationException(Resources.Resources.NotaFiscal_E_Empresa_NaoEncontrada);
        }

        // Junção do valor total das notas fiscais
        decimal totalBruto = notasFiscais.Sum(nf => nf.ValorBruto);
        if (totalBruto > empresa.LimiteCredito)
        {
            throw new InvalidOperationException(string.Format(Resources.Resources.NotaFiscal_Execede_Limite_Credito, totalBruto, empresa.LimiteCredito));
        }

        //  Cálculo da Antecipação
        var resultadoDto = new AntecipacaoResultadoDto
        {
            Empresa = empresa.Nome,
            CNPJ = empresa.CNPJ,
            Limite = empresa.LimiteCredito,
            TotalBruto = totalBruto,
            TotalLiquido = 0, 
            NotasFiscais = new List<NotaFiscalResultadoDto>()
        };

        foreach (var nota in notasFiscais)
        {
            var (valorLiquido , desagio) = CalcularAntecipacao(nota.ValorBruto, nota.DataVencimento, dataAtual);

            resultadoDto.NotasFiscais.Add(new NotaFiscalResultadoDto
            {
                Numero = nota.Numero,
                ValorBruto = nota.ValorBruto,
                ValorLiquido = valorLiquido
            });

            resultadoDto.TotalLiquido += valorLiquido;

            // Atualizar NF no banco
            nota.Status = Resources.Resources.StatusAntecipada;
            nota.ValorLiquido = valorLiquido;
            nota.Desagio = desagio;

            // Salvar as modificações da nota fiscal no banco com o novo status, valor líquido e deságio
            _dbContext.NotasFiscais.Update(nota);
            await _dbContext.SaveChangesAsync();
        }

        return resultadoDto;
    }

    #endregion

    #region Cálculo da Antecipação
    public (decimal ValorLiquido, decimal Desagio) CalcularAntecipacao(decimal valorBruto, DateTime dataVencimento, DateTime dataAtual)
    {
        // Prazo em dias
        int prazoEmDias = (dataVencimento.Date - dataAtual.Date).Days;

        // Se o prazo for zero ou negativo, não há antecipação válida
        if (prazoEmDias <= 0)
        {
            return (valorBruto, 0m);
        }

        // Prazo em meses (fração)
        double prazoEmMeses = (double)prazoEmDias / 30.0;

        // Fator de Desconto (1 + TaxaMensal)^(PrazoEmMeses)
        double fatorDesconto = (double)(1 + TaxaMes);
        double denominador = Math.Pow(fatorDesconto, prazoEmMeses);

        // Valor Líquido (Valor Presente)
        decimal valorLiquido = valorBruto / (decimal)denominador;

        // Deságio
        decimal desagio = valorBruto - valorLiquido;

        // Arredondamento para duas casas decimais
        valorLiquido = Math.Round(valorLiquido, 2);
        desagio = Math.Round(desagio, 2);

        return (valorLiquido, desagio);
    }

    #endregion

}
