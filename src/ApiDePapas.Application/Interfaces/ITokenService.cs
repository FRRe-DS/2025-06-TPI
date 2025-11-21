namespace ApiDePapas.Application.Interfaces
{
    /// <summary>
    /// Define el contrato para un servicio que gestiona
    /// la obtención de tokens de autenticación.
    /// </summary>
    public interface ITokenService
    {
        Task<string> GetAccessTokenAsync();
    }
}