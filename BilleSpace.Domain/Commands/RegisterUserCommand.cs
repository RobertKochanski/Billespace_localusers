using BilleSpace.Domain.Authentication;
using BilleSpace.Domain.Results;
using BilleSpace.Infrastructure;
using BilleSpace.Infrastructure.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BilleSpace.Domain.Commands
{
    public class RegisterUserCommand : IRequest<Result<string>>
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsReceptionist { get; set; }
    }

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<string>>
    {
        private readonly ITokenGenerator _token;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RegisterUserCommandHandler> _logger;

        public RegisterUserCommandHandler(ILogger<RegisterUserCommandHandler> logger, UserManager<User> userManager, ITokenGenerator token)
        {
            _token = token;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.PhoneNumber))
            {
                _logger.LogError($"[{DateTime.UtcNow}] Fill all fields!");
                return Result.BadRequest<string>(new List<string> { "Fill all fields!" });
            }

            var user = new User
            {
                Email = request.Email,
                UserName = request.UserName,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                IsReceptionist = request.IsReceptionist,
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                _logger.LogError(string.Join(" ", result.Errors.Select(x => x.Description)));
                return Result.BadRequest<string>(result.Errors.Select(x => x.Description).ToList());
            }

            var token = _token.CreateToken(user);

            return Result.Ok<string>(token);
        }
    }
}
