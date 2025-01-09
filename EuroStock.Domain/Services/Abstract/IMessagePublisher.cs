namespace EuroStock.Domain.Services.Abstract
{
    public interface IMessagePublisher
    {
        Task Publish<T>(T message, int attempts = 3)
            where T : class;
    }
}