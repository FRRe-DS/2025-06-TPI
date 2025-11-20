using ApiDePapas.Domain.Entities;

namespace ApiDePapas.Application.Utils
{
    public static class ShippingStatusParser
    {
        public static bool TryParse(string? input, out ShippingStatus? result)
        {
            result = null;

            if (string.IsNullOrWhiteSpace(input))
                return true;

            var snake = input.Trim()
                .Replace("-", "_")
                .Replace(" ", "_");

            if (Enum.TryParse<ShippingStatus>(snake, true, out var parsed))
            {
                result = parsed;
                return true;
            }

            var parts = snake.Split('_', StringSplitOptions.RemoveEmptyEntries);
            var pascal = string.Concat(parts.Select(p =>
                char.ToUpper(p[0]) + (p.Length > 1 ? p[1..] : "")));

            if (Enum.TryParse<ShippingStatus>(pascal, true, out parsed))
            {
                result = parsed;
                return true;
            }

            return false;
        }
    }
}