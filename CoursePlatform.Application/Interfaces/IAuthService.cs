using System.Threading.Tasks;
using CoursePlatform.Application.DTOs;

namespace CoursePlatform.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> Register(RegisterRequest request);
    Task<AuthResponse> Login(LoginRequest request);
}
