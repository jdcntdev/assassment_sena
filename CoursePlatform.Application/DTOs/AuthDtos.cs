namespace CoursePlatform.Application.DTOs;

public record LoginRequest(string Email, string Password);
public record RegisterRequest(string Email, string Password, string FullName, string Role = "Admin");
public record AuthResponse(string Token, string Email, string FullName, string Role = "Admin");
