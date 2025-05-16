﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using PennyPilot.Backend.Domain.Interfaces;

namespace PennyPilot.Backend.Domain.Services
{
    public class SecurityService : ISecurityService
    {
        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == hashedPassword;
        }
    }
}
