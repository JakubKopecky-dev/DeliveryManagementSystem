using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using UserService.Application.DTOs.Auth;
using UserService.Application.DTOs.User;
using UserService.Application.Interfaces.Jwt;
using UserService.Application.Interfaces.Services;
using UserService.Domain.Types;
using UserService.Infrastructure.Auth;
using UserService.Infrastructure.Identity;
using UserService.Infrastructure.Interfaces.Repositories;

namespace UserService.Infrastructure.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager,IJwtGenerator jwtGenerator, ILogger<AuthService> logger, IRefreshTokecRepository refreshTokecRepository) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IJwtGenerator _jwtGenerator = jwtGenerator;
        private readonly ILogger<AuthService> _logger = logger;
        private readonly IRefreshTokecRepository _refreshTokecRepository = refreshTokecRepository;



        public async Task<AuthResponseDto?> RegisterUserAsync(AuthRegisterDto authRegisterDto)
        {
            _logger.LogInformation("Registering user. UserEmail: {Email}.", authRegisterDto.Email);

            ApplicationUser newUser = new()
            {
                UserName = authRegisterDto.Email,
                Email = authRegisterDto.Email,
            };

            var result = await _userManager.CreateAsync(newUser, authRegisterDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);

                IEnumerable<string> roles = await _userManager.GetRolesAsync(newUser);
                string token = _jwtGenerator.GenerateToken(newUser.Id, newUser.Email, newUser.UserName, roles);

                string refreshToken = RefreshTokenHelper.Generate();

                await _refreshTokecRepository.InsertAsync(new()
                {
                    UserId = newUser.Id,
                    TokenHash = RefreshTokenHelper.Hash(refreshToken),
                    ExpiresAt = DateTime.UtcNow.AddDays(14)
                });

                _logger.LogInformation("User registred. UserEmail: {Email}, UserId: {Id}.", newUser.Email, newUser.Id);

                return new(newUser.UserToUserDto(), token,refreshToken);
            }

            _logger.LogWarning("User wasn't registred. UserEmail: {Email}", authRegisterDto.Email);

            return null;
        }



        public async Task<AuthResponseDto?> LoginUserAsync(AuthLoginDto authLoginDto)
        {
            _logger.LogInformation("Logging in user. UserEmail: {Email}.", authLoginDto.Email);

            ApplicationUser? user = await _userManager.FindByEmailAsync(authLoginDto.Email);
            if (user is null)
            {
                _logger.LogWarning("Login failed. User not found. UserEmail: {Email}.", authLoginDto.Email);
                return null;
            }

            bool isPasswrodValid = await _userManager.CheckPasswordAsync(user, authLoginDto.Password);
            if (!isPasswrodValid)
            {
                _logger.LogWarning("Login failed. Invalid password. UserEmail: {Email}.", authLoginDto.Email);
                return null;
            }

            IEnumerable<string> roles = await _userManager.GetRolesAsync(user);
            string token = _jwtGenerator.GenerateToken(user.Id, user.Email!, user.UserName!, roles);

            string refreshToken = RefreshTokenHelper.Generate();
            await _refreshTokecRepository.InsertAsync(new()
            {
                UserId = user.Id,
                TokenHash = RefreshTokenHelper.Hash(refreshToken),
                ExpiresAt = DateTime.UtcNow.AddDays(14)
            });

            _logger.LogInformation("User successfully logged in. UserEmail: {Email}.", authLoginDto.Email);

            return new(user.UserToUserDto(), token,refreshToken);
        }



        public async Task<UserDto?> GetCurrentUserAsync(ClaimsPrincipal claimsPrincipal)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(claimsPrincipal);

            return user?.UserToUserDto();
        }



        public async Task<AuthResponseDto> RefreshAsync(string refreshToken)
        {
            string hash = RefreshTokenHelper.Hash(refreshToken);

            var storedToken = await _refreshTokecRepository.GetStoredTokenAsync(hash);

            if (storedToken is null || !storedToken.IsActive || storedToken.User is null)
                throw new UnauthorizedAccessException();


            storedToken.RevokedAt = DateTime.UtcNow;

            var roles = await _userManager.GetRolesAsync(storedToken.User);

            string newJwt = _jwtGenerator.GenerateToken(
                storedToken.User.Id,
                storedToken.User.Email!,
                storedToken.User.UserName!,
                roles);

            string newRefreshToken = RefreshTokenHelper.Generate();

            await _refreshTokecRepository.InsertAsync(new()
            {
                UserId = storedToken.UserId,
                TokenHash = RefreshTokenHelper.Hash(newRefreshToken),
                ExpiresAt = DateTime.UtcNow.AddDays(14)
            });

            return new(storedToken.User.UserToUserDto(), newJwt,newRefreshToken);

        }



    }
}
