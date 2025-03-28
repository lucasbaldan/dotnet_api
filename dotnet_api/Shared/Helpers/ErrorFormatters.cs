using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace dotnet_api.Shared.Helpers;

public static class ErrorFormatters
{
    public static IEnumerable<string> FormatarErrosIdentity(IdentityResult result)
    {
        return result.Errors.Select(e =>
        {
            if (e.Description.Contains("already taken", StringComparison.OrdinalIgnoreCase))
                return "Usuário já cadastrado na plataforma";

            if (e.Description.Contains("Passwords must be at least", StringComparison.OrdinalIgnoreCase))
                return "A senha deve conter no mínimo 4 caracteres";

            if (e.Description.Contains("Passwords must have at least one uppercase", StringComparison.OrdinalIgnoreCase))
                return "A senha deve conter pelo menos uma letra maiúscula";

            if (e.Description.Contains("Passwords must have at least one digit", StringComparison.OrdinalIgnoreCase))
                return "A senha deve conter pelo menos um número";

            if (e.Description.Contains("Passwords must have at least one non-alphanumeric character", StringComparison.OrdinalIgnoreCase))
                return "A senha deve conter pelo menos um caractere especial (@, #, !, etc.)";

            return e.Description;

        }).ToList();
    }

    public static IEnumerable<string> FormatarErrosModel(ModelStateDictionary modelState)
    {
        return modelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .Where(msg => !string.IsNullOrWhiteSpace(msg))
            .ToList();
    }
}
