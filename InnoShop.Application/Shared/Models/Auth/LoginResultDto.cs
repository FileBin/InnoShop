﻿namespace InnoShop.Application.Shared.Models.Auth;

public class LoginResultDto {
    public required string Token { get; set; }
    public required string Username { get; set; }
}
