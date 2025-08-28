using System.Net.Http;
using System.Threading.Tasks;

namespace InventoryService.Services.HttpClients
{
    public interface ICylinderApi
    {
        Task<HttpResponseMessage> GetAllAsync();
    }
}
