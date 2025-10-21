using System.Threading.Tasks;

namespace ApiDePapas.Application.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message, string exchangeName);
    }
}
