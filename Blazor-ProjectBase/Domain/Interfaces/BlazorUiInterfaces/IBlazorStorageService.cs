using Domain.Enums;

namespace Domain.Interfaces.BlazorUiInterfaces
{
    public interface IBlazorStorageService
    {
        Task SetToProtectedStorage(ProtectedStorageVariable key, string value);
        Task<string?> GetFromProtectedStorage(ProtectedStorageVariable key);
        Task RemoveFromProtectedStorage(ProtectedStorageVariable key);
        Task SetToProtectedSession(ProtectedSessionVariable key, string value);
        Task<string?> GetFromProtectedSession(ProtectedSessionVariable key);
        Task RemoveFromProtectedSession(ProtectedSessionVariable key);
    }
}
