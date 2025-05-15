using Microsoft.AspNetCore.Components;
using MudBlazor;
using Toems_UI.Site.Layout;
namespace Toems_UI.Site;

public class BaseComponent : ComponentBase
{
    [CascadingParameter] public MainLayout? Layout { get; set; }
    public bool TitleIsSet { get; set; } = false;
    
    protected bool SetTitleAndBreadcrumbs(string title, List<BreadcrumbItem> breadCrumbs)
    {
        if (Layout is null ||  TitleIsSet) return false;
        Layout?.UpdateAppBar(title,breadCrumbs);
        TitleIsSet = true;
        return true;
    }
}