using InventoryService.Common;
using OrderService.Models.DTOs;

namespace OrderService.Services.HttpClients
{
    public interface ITransactionApiClient
    {
        Task<Result<TransactionResponseDTO>> CreateTransactionAsync(CreateTransactionDTO dto);

    }
}
