
using CaseSize.DTO;
using CaseSize.Entitades;

namespace CaseSize.Interface
{
    public interface INotasFiscais
    {
        Task<NotaFiscal> CadastrarNotaFiscal(NotaFiscalDto dto);
        Task<NotaFiscal?> GetNotaFiscalById(int id);
    }
}
