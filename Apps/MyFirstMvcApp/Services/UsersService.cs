﻿using BattleCards.Data;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Security.Cryptography;
using SUS.MvcFramework;

namespace BattleCards.Services
{
    public class UsersService : IUsersService
    {
        private readonly ApplicationDbContext db;
        public UsersService()
        {
            this.db = new ApplicationDbContext();
        }

        public void CreateUser(string username, string email, string password)
        {
            var user = new User
            {
                Username = username,
                Email = email,
                Role = IdentityRole.User,
                Password = ComputeHash(password),
            };
            this.db.Users.Add(user);
            this.db.SaveChanges();
        }

        public bool IsEmailAvailable(string email)
        {
            return !this.db.Users.Any(x => x.Email == email);
        }

        public bool IsUsernameAvailable(string username)
        {
            return !this.db.Users.Any(x => x.Username == username);
        }

        public bool IsUserValid(string username, string password)
        {
            var user = this.db.Users.FirstOrDefault(x => x.Username == username);

            return user.Password == ComputeHash(password);
        }

        private static string ComputeHash(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            using (var hash = SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                var hashedInputStringBuilder = new StringBuilder(128);

                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                return hashedInputStringBuilder.ToString();
            }
        }
    }
}
