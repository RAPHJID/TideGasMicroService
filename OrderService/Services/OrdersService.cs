using AutoMapper;
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

        public OrdersService(
            AppDbContext context,
            IMapper mapper,
            IInventoryApiClient inventoryClient,
            ICustomerApiClient customerClient)
        {
            _context = context;
            _mapper = mapper;
            _inventoryClient = inventoryClient;
            _customerClient = customerClient;
        }

        public async Task<IEnumerable<OrderReadDTO>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders.ToListAsync();
            var orderDtos = new List<OrderReadDTO>();

            foreach (var order in orders)
            {
                var dto = _mapper.Map<OrderReadDTO>(order);

                // Fetch data from other microservices
                var customer = await _customerClient.GetCustomerByIdAsync(order.CustomerId);
                var cylinder = await _inventoryClient.GetCylinderByIdAsync(order.CylinderId);

                dto.CustomerName = customer?.Name ?? "Unknown";
                dto.CylinderName = cylinder?.CylinderName ?? "Unknown";

                orderDtos.Add(dto);
            }

            return orderDtos;
        }

        public async Task<OrderReadDTO?> GetOrderByIdAsync(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return null;

            var dto = _mapper.Map<OrderReadDTO>(order);

            var customer = await _customerClient.GetCustomerByIdAsync(order.CustomerId);
            var cylinder = await _inventoryClient.GetCylinderByIdAsync(order.CylinderId);

            dto.CustomerName = customer?.Name ?? "Unknown";
            dto.CylinderName = cylinder?.CylinderName ?? "Unknown";

            return dto;
        }

        public async Task<OrderReadDTO> CreateOrderAsync(OrderCreateDTO dto)
        {
            // Check stock with Inventory service
            var inStock = await _inventoryClient.CheckStockAsync(dto.CylinderId, dto.Quantity);

            // 🔍 DEBUG LINE - check what we are sending & receiving
            Console.WriteLine($"DEBUG CHECK-STOCK | CylinderId: {dto.CylinderId} | Quantity: {dto.Quantity} | Result: {inStock}");

            if (!inStock)
                throw new Exception("Not enough stock available in InventoryService.");


            var order = _mapper.Map<Order>(dto);

            if (Enum.TryParse<OrderStatus>(dto.Status, true, out var status))
                order.Status = status;
            else
                order.Status = OrderStatus.Pending;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var orderDto = _mapper.Map<OrderReadDTO>(order);

            var customer = await _customerClient.GetCustomerByIdAsync(order.CustomerId);
            var cylinder = await _inventoryClient.GetCylinderByIdAsync(order.CylinderId);

            orderDto.CustomerName = customer?.Name ?? "Unknown";
            orderDto.CylinderName = cylinder?.CylinderName ?? "Unknown";

            return orderDto;
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid id, string newStatus)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return false;

            if (Enum.TryParse<OrderStatus>(newStatus, true, out var status))
            {
                order.Status = status;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
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

