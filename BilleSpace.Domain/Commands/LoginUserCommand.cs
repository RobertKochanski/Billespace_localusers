using BilleSpace.Domain.Authentication;
using BilleSpace.Domain.Results;
using BilleSpace.Infrastructure;
using BilleSpace.Infrastructure.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilleSpace.Domain.Commands
{
    public class LoginUserCommand : IRequest<Result<string>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<string>>
    {
        private readonly ITokenGenerator _token;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<LoginUserCommandHandler> _logger;

        public LoginUserCommandHandler(ILogger<LoginUserCommandHandler> logger, UserManager<User> userManager, ITokenGenerator token)
        {
            _token = token;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                _logger.LogError($"[{DateTime.UtcNow}] Can not find user with Email {request.Email}");
                return Result.BadRequest<string>(new List<string> { $"[{DateTime.UtcNow}] Can not find user with Email {request.Email}" });
            }

            // password check

            var token = _token.CreateToken(user);

            return Result.Ok<string>(token);
        }
    }
}
