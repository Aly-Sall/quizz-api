using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using Microsoft.AspNetCore.Identity;

namespace _Net6CleanArchitectureQuizzApp.Infrastructure.Identity;

public static class IdentityResultExtensions
{
    public static Result ToApplicationResult(this IdentityResult result)
    {
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(ListToArray(result.Errors));
    }
    public static string[] ListToArray(IEnumerable<IdentityError> errors)
    {
        List<string> returnedarray = new List<string> (); 
        var array = errors.ToArray();
        for (int i = 0; i < array.Length; i++) 
        {
            returnedarray[i] = array[i].Description; 
        }
        return returnedarray.ToArray(); 
    }
}
