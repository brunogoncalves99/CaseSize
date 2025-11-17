using CaseSize.Context;
using CaseSize.DTO;
using CaseSize.Entidades;
using CaseSize.Entitades;
using Microsoft.EntityFrameworkCore;

namespace CaseSize.Service;


public class EmpresaService
{

    private readonly AppDbContext _dbContext;

    public EmpresaService(AppDbContext dbContext)
    {
        _dbContext = dbContext; 
    }

    #region Cadastramento de Empresa
    public async Task<Empresa> CadastrarEmpresa(EmpresaDto dto)
    {
        // Verificar se CNPJ já existe
        if (await _dbContext.Empresas.AnyAsync(e => e.CNPJ == dto.CNPJ))
        {
            throw new InvalidOperationException(Resources.Resources.CNPJExistente);
        }

        if (!CnpjValido(dto.CNPJ))
        {
            throw new InvalidOperationException(Resources.Resources.CNPJExistente);
        }

        decimal limite = CalcularLimite(dto.FaturamentoMensal, dto.Ramo);

        var empresa = new Empresa
        {
            CNPJ = dto.CNPJ,
            Nome = dto.Nome,
            FaturamentoMensal = dto.FaturamentoMensal,
            Ramo = dto.Ramo,
            LimiteCredito = limite
        };

        _dbContext.Empresas.Add(empresa);
        await _dbContext.SaveChangesAsync();

        return empresa;
    }

    #endregion

    #region Validar o CNPJ

    public static bool CnpjValido(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return false;

        // Remove caracteres especiais
        cnpj = new string(cnpj.Where(char.IsDigit).ToArray());

        if (cnpj.Length != 14)
            return false;

        var invalidos = new[]
        {
            "00000000000000",
            "11111111111111",
            "22222222222222",
            "33333333333333",
            "44444444444444",
            "55555555555555",
            "66666666666666",
            "77777777777777",
            "88888888888888",
            "99999999999999"
        };

        if (invalidos.Contains(cnpj))
            return false;

        // Calcula dígito verificador
        int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCnpj = cnpj.Substring(0, 12);
        int soma = 0;

        for (int i = 0; i < 12; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

        int resto = soma % 11;
        int digito1 = resto < 2 ? 0 : 11 - resto;

        tempCnpj += digito1;
        soma = 0;

        for (int i = 0; i < 13; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        int digito2 = resto < 2 ? 0 : 11 - resto;

        return cnpj.EndsWith(digito1.ToString() + digito2.ToString());
    }

    #endregion

    #region Calculo do Limite de Crédito
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

    #endregion

    #region Busca a Empresa pelo Id
    public async Task<Empresa?> GetEmpresaById(int id) 
    {
        return await _dbContext.Empresas.FindAsync(id);
    }
    #endregion


}
