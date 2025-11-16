using System.Net.NetworkInformation;
using System.Reflection;

namespace CaseSize.Resources;

public static class Resources
{
    /// <summary>
    /// Todos os detalhamentos de erros e mensagens fixas do sistema
    /// </summary>
    public static string EmpresaNaoCadastrada
    {
        get => "Não existe cadastado para a empresa informada";
    }
    public static string NotaFiscalVencida
    {
        get => "A Data de Vencimento deve ser maior que a data atual.";
    }
    public static string StatusPendente
    {
        get => "Pendente";
    }
    public static string StatusAntecipada
    {
        get => "Antecipada";
    }
    public static string EmpresaNaoEncontrada
    {
        get => "Empresa não encontrada";
    }
    public static string NotaFiscal_E_Empresa_NaoEncontrada
    {
        get => "Uma ou mais notas fiscais não foram encontradas, não pertencem à empresa ou já foram antecipadas";
    }
    public static string NotaFiscal_Execede_Limite_Credito
    {
        get => "O valor total da(s) nota(s) fiscal(is) informada(s) ({0:C}) excede o limite de crédito da empresa ({1:C}).";
    }
    public static string CNPJExistente
    {
        get => "Empresa com este CNPJ já cadastrada.";
    }
    public static string ErroCadastroEmpresa
    {
        get => "Erro ao cadastrar empresa, consulte detalhes";
    }
    public static string ErroProcessarAntecipacao
    {
        get => "Erro ao processar antecipação, consulte detalhes.";
    }
    public static string ErroCadastroNotaFiscal
    {
        get => "Erro ao cadastrar nota fiscal, consulte detalhes";
    }
    public static string ErroProcessamentoAntecipacao
    {
        get => "Nenhuma nota fiscal selecionada para antecipação.";
    }
    public static string Empresa_NotasFiscais_NaoEncontradas
    {
        get => "Empresa ou notas fiscais não encontradas.";
    }

}
