﻿using Model.DB;

namespace DAL.Interface
{
    public interface IUnitOfWork
    {
        IGenericRepository<Role> RoleRepo { get; }
        IGenericRepository<User> UserRepo { get; }
        IGenericRepository<Good> GoodRepo { get; }
        IGenericRepository<Category> CategoryRepo { get; }
        IGenericRepository<Property> PropertyRepo { get; }
        IGenericRepository<WebShop> WebShopRepo { get; }
        IGenericRepository<ParserTask> ParserRepo { get; }

        void Dispose();

        void Save();
    }
}