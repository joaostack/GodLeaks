using GodLeaks.Domain.Entities.LeakSightAPI;

namespace GodLeaks.Application.Interfaces;

public interface ILeakSight
{
    Task<Username.Root> SearchUsername(string username);
    Task<Email.Root> SearchEmail(string email);
    Task<CPF.Root> SearchCPF(string cpf);
    Task<Placa.Root> SearchPlaca(string placa);
    Task<Nome.Root> SearchNome(string nome);
    Task<Password.Root> SearchPassword(string password);
    Task<Url.Root> SearchUrl(string url);
}