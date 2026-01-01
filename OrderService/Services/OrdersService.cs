using AutoMapper;
using InventoryService.Common;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using OrderService.Models.DTOs;
using OrderService.Models.Enums;
using OrderService.Services.HttpClients;
using OrderService.Services.IServices;

namespace OrderService.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IInventoryApiClient _inventoryClient;
        private readonly ICustomerApiClient _customerClient;
        private readonly ICylinderApiClient _cylinderClient;
        private readonly ITransactionApiClient _transactionClient;


        public OrdersService(
            AppDbContext context,
            IMapper mapper,
            IInventoryApiClient inventoryClient,
            ICustomerApiClient customerClient,
            ICylinderApiClient cylinderClient,
            ITransactionApiClient transactionClient)
            
        {
            _context = context;
            _mapper = mapper;
            _inventoryClient = inventoryClient;
            _customerClient = customerClient;
            _cylinderClient = cylinderClient;
            _transactionClient = transactionClient;
        }

        public async Task<IEnumerable<OrderReadDTO>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders.ToListAsync();
            var result = new List<OrderReadDTO>();

            foreach (var order in orders)
            {
                var dto = _mapper.Map<OrderReadDTO>(order);

                var customer = await _customerClient.GetCustomerByIdAsync(order.CustomerId);
                dto.CustomerName = customer?.FullName ?? "Unknown";

                var cylinder = await _cylinderClient.GetByIdAsync(order.CylinderId);

                dto.CylinderName = cylinder == null
                    ? "Unknown"
                    : $"{cylinder.Brand} {cylinder.Size}";

                // Orders should NOT depend on Inventory data
                //dto.CylinderName = "N/A";

                result.Add(dto);
            }

            return result;
        }

        public async Task<OrderReadDTO?> GetOrderByIdAsync(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return null;

            var dto = _mapper.Map<OrderReadDTO>(order);

            var customer = await _customerClient.GetCustomerByIdAsync(order.CustomerId);
            dto.CustomerName = customer?.FullName ?? "Unknown";

            var cylinder = await _cylinderClient.GetByIdAsync(order.CylinderId);

            dto.CylinderName = cylinder == null
                ? "Unknown"
                : $"{cylinder.Brand} {cylinder.Size}";
            //dto.CylinderName = "N/A";

            return dto;
        }

        public async Task<OrderReadDTO> CreateOrderAsync(OrderCreateDTO dto)
        {
            await using var dbTransaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Check stock
                var stockCheck = await _inventoryClient.CheckStockAsync(dto.CylinderId, dto.Quantity);
                if (!stockCheck.IsSuccess)
                    throw new Exception("Not enough stock available.");

                // 2. Create order (not committed yet)
                var order = _mapper.Map<Order>(dto);
                order.Status = OrderStatus.Pending;

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // 3. Decrease stock
                var decreaseResult = await _inventoryClient.DecreaseStockAsync(dto.CylinderId, dto.Quantity);
                if (!decreaseResult.IsSuccess)
                    throw new Exception("Stock decrease failed.");

                // 4. Create transaction
                var transactionDto = new CreateUpdateTransactionDTO
                {
                    CustomerId = order.CustomerId,
                    CylinderId = order.CylinderId,
                    Amount = order.TotalPrice
                };

                var transactionResult = await _transactionClient.CreateTransactionAsync(transactionDto);
                if (!transactionResult.IsSuccess)
                    throw new Exception($"Transaction creation failed: {transactionResult.Error}");

                // 5. Finalize order
                order.Status = OrderStatus.Completed;
                await _context.SaveChangesAsync();

                await dbTransaction.CommitAsync();

                var result = _mapper.Map<OrderReadDTO>(order);
                var customer = await _customerClient.GetCustomerByIdAsync(order.CustomerId);
                result.CustomerName = customer?.FullName ?? "Unknown";

                return result;
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }




        public async Task<bool> UpdateOrderStatusAsync(Guid id, string newStatus)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return false;

            if (!Enum.TryParse<OrderStatus>(newStatus, true, out var status))
                return false;

            order.Status = status;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteOrderAsync(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
