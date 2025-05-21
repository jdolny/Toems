using Microsoft.AspNetCore.Components;
using MudBlazor;
using Toems_UI.Site.Layout;
namespace Toems_UI.Site;

public class ComputerBreadcrumbs(string computerName = "")
{
    private BreadcrumbItem _home = new("Home", href: "/");
    private BreadcrumbItem _manage = new("Manage Computers", "/computers/manage");
    private BreadcrumbItem _computerName = new(computerName, href:null,disabled: true);


    public List<BreadcrumbItem> Attachments()
    {
        return [_home, _manage, _computerName, new("Attachments", href: null, disabled: true)];
    }
    public List<BreadcrumbItem> Certificates()
    {
        return [_home, _manage, _computerName, new("Certificates", href: null, disabled: true)];
    }
    public List<BreadcrumbItem> Comments()
    {
        return [_home, _manage, _computerName, new("Comments", href: null, disabled: true)];
    }
    public List<BreadcrumbItem> CustomAttributes()
    {
        return [_home, _manage, _computerName, new("Custom Attributes", href: null, disabled: true)];
    }
    public List<BreadcrumbItem> CustomInventory()
    {
        return [_home, _manage, _computerName, new("Custom Inventory", href: null, disabled: true)];
    }
    public List<BreadcrumbItem> EffectivePolicy()
    {
        return [_home, _manage, _computerName, new("Effective Policy", href: null, disabled: true)];
    }
    public List<BreadcrumbItem> General()
    {
        return [_home, _manage, _computerName, new("General", href: null, disabled: true)];
    }
    public List<BreadcrumbItem> ImageSettings()
    {
        return [_home, _manage, _computerName, new("Image Settings", href: null, disabled: true)];
    }
    public List<BreadcrumbItem> ImagingLogs()
    {
        return [_home, _manage, _computerName, new("Imaging Logs", href: null, disabled: true)];
    }
    public List<BreadcrumbItem> OnDemandModule()
    {
        return [_home, _manage, _computerName, new("On-Demand Module", href: null, disabled: true)];
    }
    public List<BreadcrumbItem> PolicyHistory()
    {
        return [_home, _manage, _computerName, new("Policy History", href: null, disabled: true)];
    }
    public List<BreadcrumbItem> Software()
    {
        return [_home, _manage, _computerName, new("Software", href: null, disabled: true)];
    }
    public List<BreadcrumbItem> SystemInformation()
    {
        return [_home, _manage, _computerName, new("System Information", href: null, disabled: true)];
    }
    public List<BreadcrumbItem> Manage()
    {
        return [_home, _manage];
    }
}