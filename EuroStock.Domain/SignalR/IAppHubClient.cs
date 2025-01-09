using EuroStock.Domain.Models;

namespace EuroStock.Domain.SignalR;

public interface IAppHubClient
{
    Task UploadImage(UploadImageResult updatedRoutes);
}