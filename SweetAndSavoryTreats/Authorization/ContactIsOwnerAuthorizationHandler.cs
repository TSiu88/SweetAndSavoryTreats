using System.Threading.Tasks;
using SweetSavoryTreats.Data;
using SweetSavoryTreats.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace SweetSavoryTreats.Authorization
{
  public class ContactIsOwnerAuthorizationHandler
    : AuthorizationHandler<OperationAuthorizationRequirement, Contact>
  {
    UserManager<IdentityUser> _userManager;

    public ContactIsOwnerAuthorizationHandler(UserManager<IdentityUser> userManager)
    {
      _userManager = userManager;
    }
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, 
        Contact resource)
    {
      if (context.User == null || resource == null)
      {
        return Task.CompletedTask;
      }

      if (requirement.Name != Constants.CreateOperationName &&
          requirement.Name != Constants.ReadOperationName   &&
          requirement.Name != Constants.UpdateOperationName &&
          requirement.Name != Constants.DeleteOperationName )
      {
        return Task.CompletedTask;
      }

      // Administrators can do anything.
      if (resource.OwnerID == _userManager.GetUserId(context.User))
      {
        context.Succeed(requirement);
      }

      return Task.CompletedTask;
    }
  }
}