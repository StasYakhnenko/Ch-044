﻿using Model.DTO;
using System.Collections.Generic;

namespace BAL.Interface
{
    public interface IUserManager
    {
        void UpdateUser(int Id, string UserName, string Password, string Email, int RoleId);

        UserDTO GetUser(string email, string password);
        NetworkUserDTO GetNetworkUser(string networkAccountId, string network);

        List<UserDTO> GetAll();

        void Insert(UserDTO user);
        void Insert(NetworkUserDTO user);

        bool EmailIsExist(string email);
        bool NetworkAccountExict(string networkAccountId, string network);
    }
}