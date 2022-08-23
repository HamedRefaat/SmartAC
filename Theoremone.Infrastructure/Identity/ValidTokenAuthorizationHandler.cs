using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Theoremone.SmartAc.Application.Common.Interfaces.Repository;
using Theoremone.SmartAc.Infrastructure.Persistence;

namespace Theoremone.SmartAc.Infrastructure.Identity;

public class ValidTokenAuthorizationHandler : AuthorizationHandler<ValidTokenRequirement>
{
    private readonly IDeviceRegistrationRepo _deviceRegistrationRepo;

    public ValidTokenAuthorizationHandler(IDeviceRegistrationRepo deviceRegistrationRepo)
    {
        _deviceRegistrationRepo = deviceRegistrationRepo;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ValidTokenRequirement requirement)
    {
        var tokenId = context.User.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Jti)?.Value;
        var deviceSerialNumber = context.User.Identity?.Name;

        var isTokenValid = await _deviceRegistrationRepo.IsActiveRegisterdDeviceWithToken(deviceSerialNumber, tokenId);

        if (isTokenValid)
        {
            context.Succeed(requirement);
        }
    }
}
