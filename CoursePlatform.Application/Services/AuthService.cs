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
    RoleManager<IdentityRole<Guid>> roleManager,
    IConfiguration configuration) : IAuthService
{
    // Known roles
    public const string RoleAdmin = "Admin";
    public const string RoleUser  = "User";

    public async Task<AuthResponse> Register(RegisterRequest request)
    {
        // Ensure roles exist
        await EnsureRoles();

        var user = new ApplicationUser
        {
            UserName  = request.Email,
            Email     = request.Email,
            FullName  = request.FullName
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        // Assign role (default Admin for normal registration, User for regular accounts)
        var role = (request.Role == RoleUser) ? RoleUser : RoleAdmin;
        await userManager.AddToRoleAsync(user, role);

        var token = await GenerateJwtToken(user, role);
        return new AuthResponse(token, user.Email!, user.FullName, role);
    }

    public async Task<AuthResponse> Login(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
            throw new Exception("Credenciales incorrectas");

        var roles = await userManager.GetRolesAsync(user);
        var role = roles.Contains(RoleUser) ? RoleUser : RoleAdmin;

        var token = await GenerateJwtToken(user, role);
        return new AuthResponse(token, user.Email!, user.FullName, role);
    }

    private async Task EnsureRoles()
    {
        foreach (var r in new[] { RoleAdmin, RoleUser })
        {
            if (!await roleManager.RoleExistsAsync(r))
                await roleManager.CreateAsync(new IdentityRole<Guid>(r));
        }
    }

    private Task<string> GenerateJwtToken(ApplicationUser user, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new Claim("FullName", user.FullName),
            new Claim(ClaimTypes.Role, role),
        };

        var secretKey = configuration["Jwt:Secret"] ?? throw new Exception("JWT Secret not found");
        var key       = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds     = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires   = DateTime.Now.AddDays(Convert.ToDouble(configuration["Jwt:ExpireDays"] ?? "7"));

        var token = new JwtSecurityToken(
            configuration["Jwt:Issuer"],
            configuration["Jwt:Audience"],
            claims,
            expires: expires,
            signingCredentials: creds
        );

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }
}
