using System.Text.Json;
using MudBlazor;

public static class Constants
{
    public static readonly DialogOptions DeleteDialogOptions = new DialogOptions
    {
        CloseButton = false,
        MaxWidth = MaxWidth.Medium,
        FullWidth = true
    };
    
    public const int DefaultPageSize = 50;
    
  
}