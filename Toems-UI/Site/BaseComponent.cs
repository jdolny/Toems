using Microsoft.AspNetCore.Components;
using MudBlazor;
using Toems_UI.Site.Layout;
namespace Toems_UI.Site;

public class BaseComponent : ComponentBase
{
    [CascadingParameter] public MainLayout? Layout { get; set; }
    protected bool ParametersSet { get; set; } = false;
    
    protected bool SetTitleAndBreadcrumbs(string title, List<BreadcrumbItem> breadCrumbs)
    {
        if (ParametersSet) return true;
        if (Layout is null) return false;
        Layout?.UpdateAppBar(title,breadCrumbs);
        ParametersSet = true;
        return true;
    }
    
    
}