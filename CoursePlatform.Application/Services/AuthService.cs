using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using CoursePlatform.Application.DTOs;
using CoursePlatform.Application.Interfaces;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration) : IAuthService
{
    public async Task<AuthResponse> Register(RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        var token = GenerateJwtToken(user);
        return new AuthResponse(token, user.Email!, user.FullName);
    }

    public async Task<AuthResponse> Login(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new Exception("Invalid credentials");
        }

        var token = GenerateJwtToken(user);
        return new AuthResponse(token, user.Email!, user.FullName);
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("FullName", user.FullName)
        };

        var secretKey = configuration["Jwt:Secret"] ?? throw new Exception("JWT Secret not found");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(Convert.ToDouble(configuration["Jwt:ExpireDays"] ?? "7"));

        var token = new JwtSecurityToken(
            configuration["Jwt:Issuer"],
            configuration["Jwt:Audience"],
            claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
