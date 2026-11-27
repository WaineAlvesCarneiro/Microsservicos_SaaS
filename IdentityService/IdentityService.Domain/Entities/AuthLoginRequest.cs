namespace IdentityService.Domain.Entities;

public record AuthLoginRequest(
    string Username,
    string Password
);