﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KFCApp.AppData
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class AppConnector : DbContext
    {
        public AppConnector()
            : base("name=AppConnector")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Employees> Employees { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Ingredient> Ingredient { get; set; }
        public virtual DbSet<IngredientCategory> IngredientCategory { get; set; }
        public virtual DbSet<Dish> Dish { get; set; }
        public virtual DbSet<DishCategory> DishCategory { get; set; }
        public virtual DbSet<Blocking> Blocking { get; set; }
        public virtual DbSet<Client> Client { get; set; }
        public virtual DbSet<ClientAddress> ClientAddress { get; set; }
        public virtual DbSet<DishStatus> DishStatus { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderCompound> OrderCompound { get; set; }
        public virtual DbSet<OrderStatus> OrderStatus { get; set; }
        public virtual DbSet<OrderStatusHistory> OrderStatusHistory { get; set; }
    }
}
