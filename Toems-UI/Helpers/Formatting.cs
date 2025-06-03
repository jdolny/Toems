using System.Text.Json;
using MudBlazor;

public static class Formatting
{
    public static string FormatJson(string jsonString)
    {
        try
        {
            var jsonDocument = JsonDocument.Parse(jsonString);
            
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            return JsonSerializer.Serialize(jsonDocument, options);
        }
        catch (JsonException ex)
        {
            return $"Error formatting JSON: {ex.Message}";
        }
    }
}