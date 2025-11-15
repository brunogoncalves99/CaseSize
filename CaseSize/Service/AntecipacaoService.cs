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

    public async Task<Empresa> CadastrarEmpresa(EmpresaDto dto)
    {
        // 1. Verificar se CNPJ já existe
        if (await _dbContext.Empresas.AnyAsync(e => e.CNPJ == dto.CNPJ))
        {
            throw new InvalidOperationException("Empresa com este CNPJ já cadastrada.");
        }

        // 2. Calcular Limite de Crédito
        decimal limite = CalcularLimite(dto.FaturamentoMensal, dto.Ramo);

        // 3. Criar Entidade
        var empresa = new Empresa
        {
            CNPJ = dto.CNPJ,
            Nome = dto.Nome,
            FaturamentoMensal = dto.FaturamentoMensal,
            Ramo = dto.Ramo,
            LimiteCredito = limite
        };

        // 4. Salvar
        _dbContext.Empresas.Add(empresa);
        await _dbContext.SaveChangesAsync();

        return empresa;
    }

    public async Task<NotaFiscal> CadastrarNotaFiscal(NotaFiscalDto dto)
    {
        // Verificar se a Data de Vencimento é maior que a data atual
        if (dto.DataVencimento.Date <= DateTime.Today)
        {
            throw new InvalidOperationException("A Data de Vencimento deve ser maior que a data atual.");
        }

        // Verificar se a empresa existe caso existe antes de cadastrar a nota fiscal
        var empresa = await _dbContext.Empresas.FindAsync(dto.EmpresaId);
        if (empresa == null)
        {
            throw new KeyNotFoundException("Empresa não encontrada.");
        }

        // 3. Criar Entidade de dados NotaFiscal para salvar no banco
        var notaFiscal = new NotaFiscal
        {
            EmpresaId = dto.EmpresaId,
            Numero = dto.Numero,
            ValorBruto = dto.ValorBruto,
            DataVencimento = dto.DataVencimento.Date,
            Status = "Pendente"
        };

        // 4. Salvar
        _dbContext.NotasFiscais.Add(notaFiscal);
        await _dbContext.SaveChangesAsync();

        return notaFiscal;
    }


    // Método para calcular o limite de crédito com base no faturamento e ramo da empresa
    public decimal CalcularLimite(decimal faturamento, RamoEmpresa ramo)
    {
        decimal percentual;

        if (faturamento >= 10000.00m && faturamento <= 50000.00m)
        {
            percentual = 0.50m; // 50%
        }
        else if (faturamento >= 50001.00m && faturamento <= 100000.00m)
        {
            percentual = (ramo == RamoEmpresa.Servicos) ? 0.55m : 0.60m;
        }
        else if (faturamento >= 100001.00m)
        {
            percentual = (ramo == RamoEmpresa.Servicos) ? 0.60m : 0.65m;
        }
        else
        {
            // Faturamento abaixo de R$ 10.000,00 não entra na política de crédito
            percentual = 0.00m;
        }

        return faturamento * percentual;
    }

    public (decimal ValorLiquido, decimal Desagio) CalcularAntecipacao(decimal valorBruto, DateTime dataVencimento, DateTime dataAtual)
    {
        // 1. Prazo em dias
        int prazoEmDias = (dataVencimento.Date - dataAtual.Date).Days;

        // Se o prazo for zero ou negativo, não há antecipação válida
        if (prazoEmDias <= 0)
        {
            return (valorBruto, 0m);
        }

        // 2. Prazo em meses (fração)
        double prazoEmMeses = (double)prazoEmDias / 30.0;

        // 3. Fator de Desconto (1 + TaxaMensal)^(PrazoEmMeses)
        double fatorDesconto = (double)(1 + TaxaMes);
        double denominador = Math.Pow(fatorDesconto, prazoEmMeses);

        // 4. Valor Líquido (Valor Presente)
        decimal valorLiquido = valorBruto / (decimal)denominador;

        // 5. Deságio
        decimal desagio = valorBruto - valorLiquido;

        // Arredondamento para duas casas decimais
        valorLiquido = Math.Round(valorLiquido, 2);
        desagio = Math.Round(desagio, 2);

        return (valorLiquido, desagio);
    }

    public async Task<AntecipacaoResultadoDto> ProcessamentoAntecipacao(int idEmpresa, List<int> notasFiscaisIds)
    {
        var empresa = await _dbContext.Empresas.FindAsync(idEmpresa); 
        if (empresa == null)
        {
            throw new KeyNotFoundException("Empresa não encontrada.");
        }
        var notasFiscais = await _dbContext.NotasFiscais.Where(nf => notasFiscaisIds.Contains(nf.EmpresaId) && nf.EmpresaId == idEmpresa).ToListAsync();
        if (notasFiscais.Count != notasFiscaisIds.Count)
        {
            throw new InvalidOperationException("Uma ou mais notas fiscais não foram encontradas, não pertencem à empresa ou já foram antecipadas.");
        }

        // 2. Validação de Limite
        decimal totalBruto = notasFiscais.Sum(nf => nf.ValorBruto);
        if (totalBruto > empresa.LimiteCredito)
        {
            throw new InvalidOperationException($"O valor total das notas ({totalBruto:C}) excede o limite de crédito da empresa ({empresa.LimiteCredito:C}).");
        }

        // 3. Cálculo da Antecipação
        var resultadoDto = new AntecipacaoResultadoDto
        {
            Empresa = empresa.Nome,
            CNPJ = empresa.CNPJ,
            Limite = empresa.LimiteCredito,
            TotalBruto = totalBruto
        };

        decimal totalLiquido = 0;
        DateTime dataAtual = DateTime.Today;

        foreach (var nota in notasFiscais)
        {
            var (valorLiquido, desagio) = CalcularAntecipacao(nota.ValorBruto, nota.DataVencimento, dataAtual);
            // Atualizar total líquido
            resultadoDto.NotasFiscais.Add(new NotaFiscalResultadoDto
            {
                Numero = nota.Numero,
                ValorBruto = nota.ValorBruto,
                ValorLiquido = valorLiquido
            });

            // Atualizar total líquido
            var notaFiscal = new NotaFiscal
            {
                EmpresaId = idEmpresa,
                Numero = nota.Numero,
                ValorBruto = nota.ValorBruto,
                DataVencimento = nota.DataVencimento,
                Status = "Antecipada"
            };

            // Salvar as alterações no banco de dados
            _dbContext.NotasFiscais.Update(notaFiscal);
        }

        resultadoDto.TotalLiquido = Math.Round(totalLiquido, 2);

        return resultadoDto;

    }

    public async Task<Empresa?> GetEmpresaById(int id) // Método para obter uma empresa pelo ID
    {
        return await _dbContext.Empresas.FindAsync(id);
    }

    public async Task<NotaFiscal?> GetNotaFiscalById(int id) // Método para obter uma nota fiscal pelo ID
    {
        return await _dbContext.NotasFiscais.FindAsync(id);
    }

}
