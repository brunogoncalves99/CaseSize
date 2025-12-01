using CaseSize.DTO;
using CaseSize.Entidades;
using CaseSize.Entitades;

namespace CaseSize.Interface
{
    public interface IEmpresa
    {
        Task<Empresa> CadastrarEmpresa(EmpresaDto dto);
        decimal CalcularLimite(decimal faturamento, RamoEmpresa ramo);
        Task<Empresa?> GetEmpresaById(int id);
    }
}
