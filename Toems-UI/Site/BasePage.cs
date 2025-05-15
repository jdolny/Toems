using Microsoft.AspNetCore.Components;
using MudBlazor;
using Toems_UI.Site.Layout;
namespace Toems_UI.Site;

public class BasePage : ComponentBase
{
    [CascadingParameter] public MainLayout? Layout { get; set; }

    protected override void OnInitialized()
    {
        Layout?.ClearActionButtons();
        base.OnInitialized();
    }
    
    protected void SetTitleAndBreadcrumbs(string title, List<BreadcrumbItem> breadCrumbs)
    {
        Layout?.UpdateAppBar(title,breadCrumbs);
    }
}