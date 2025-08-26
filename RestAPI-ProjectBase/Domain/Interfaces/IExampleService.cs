using Domain.Models;

namespace Domain.Interfaces
{
    public interface IExampleService
    {
        Task<DefaultResponseModel> Example();
    }
}