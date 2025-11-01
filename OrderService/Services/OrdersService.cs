﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using OrderService.Models.DTOs;
using OrderService.Models.Enums;
using OrderService.Services.IServices;
using OrdersService.Services.HttpClients;

namespace OrderService.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IInventoryApiClient _inventoryClient; 

        public OrdersService(AppDbContext context, IMapper mapper, IInventoryApiClient inventoryClient)
        {
            _context = context;
            _mapper = mapper;
            _inventoryClient = inventoryClient;
        }

        public async Task<IEnumerable<OrderReadDTO>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders.ToListAsync();
            return _mapper.Map<IEnumerable<OrderReadDTO>>(orders);
        }

        public async Task<OrderReadDTO?> GetOrderByIdAsync(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            return order == null ? null : _mapper.Map<OrderReadDTO>(order);
        }

        public async Task<OrderReadDTO> CreateOrderAsync(OrderCreateDTO dto)
        {
            var inStock = await _inventoryClient.CheckStockAsync(dto.CylinderId, dto.Quantity);
            if (!inStock)
                throw new Exception("Not enough stock available in InventoryService.");

            var order = _mapper.Map<Order>(dto);
            if (Enum.TryParse<OrderStatus>(dto.Status, true, out var status))
                order.Status = status;
            else
                order.Status = OrderStatus.Pending;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return _mapper.Map<OrderReadDTO>(order);
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
