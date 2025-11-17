using CaseSize.Context;
using CaseSize.DTO;
using CaseSize.Entitades;

namespace CaseSize.Service;


public class NotaFiscalService
{
    private readonly AppDbContext _dbContext;

    public NotaFiscalService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region Cadastramento de Nota Fiscal
    public async Task<NotaFiscal> CadastrarNotaFiscal(NotaFiscalDto dto)
    {
        var vencimento = dto.DataVencimento.Date;
        var hoje = DateTime.Today;

        // Verificar se a Data de Vencimento é maior que a data atual
        if (vencimento <= hoje)
        {
            throw new InvalidOperationException(Resources.Resources.NotaFiscalVencida);
        }

        var empresa = await _dbContext.Empresas.FindAsync(dto.EmpresaId);
        if (empresa == null)
        {
            throw new KeyNotFoundException(Resources.Resources.EmpresaNaoCadastrada);
        }

        var notaFiscal = new NotaFiscal
        {
            EmpresaId = dto.EmpresaId,
            Numero = dto.Numero,
            ValorBruto = dto.ValorBruto,
            DataVencimento = dto.DataVencimento.Date,
            Status = Resources.Resources.StatusPendente
        };

        _dbContext.NotasFiscais.Add(notaFiscal);
        await _dbContext.SaveChangesAsync();

        return notaFiscal;
    }

    #endregion

    #region Busca de Nota Fiscal por ID
    public async Task<NotaFiscal?> GetNotaFiscalById(int id)
    {
        return await _dbContext.NotasFiscais.FindAsync(id);
    }
    #endregion

}
