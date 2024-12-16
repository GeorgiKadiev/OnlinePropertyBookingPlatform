using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.RegularExpressions;

public class XssSanitizationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is string strValue)
            {
                var sanitized = SanitizeInput(strValue);
                context.ActionArguments[context.ActionArguments.Keys.First(k => context.ActionArguments[k] == argument)] = sanitized;
            }
            else if (argument != null)
            {
                SanitizeObjectProperties(argument);
            }
        }
    }

    private void SanitizeObjectProperties(object obj)
    {
        var properties = obj.GetType().GetProperties()
                            .Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

        foreach (var prop in properties)
        {
            var value = (string)prop.GetValue(obj);
            if (!string.IsNullOrEmpty(value))
            {
                prop.SetValue(obj, SanitizeInput(value));
            }
        }
    }

    private string SanitizeInput(string input)
    {
        // Премахване на зловреден HTML код
        return Regex.Replace(input, "<.*?>", string.Empty);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Няма нужда от обработка след изпълнение на действието
    }
}
