﻿using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using System.Text;

namespace WebService.Services;

public class PasswordService
{
    
    public static string GenerateSalt(int size)
    {
        var buffer = RandomNumberGenerator.GetBytes(size);
        return Convert.ToBase64String(buffer);
    }

    public static string HashPassword(string pwd, string salt, int size)
    {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            pwd,
            Encoding.UTF8.GetBytes(salt),
            KeyDerivationPrf.HMACSHA256,
            10000,
            size));
    }
}