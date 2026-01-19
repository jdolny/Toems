using Microsoft.AspNetCore.Components;
using MudBlazor;
using Toems_Common.Entity;
using Toems_UI.Site.Layout;
namespace Toems_UI.Site;

public class ImageBreadcrumbs(EntityImage Image = null, ImageProfileWithImage Profile = null)
{
    private BreadcrumbItem _home = new("Home", href: "/");
    private BreadcrumbItem _manage = new("Manage Images", "/images/manage");
    private BreadcrumbItem _imageName = new(Image?.Name, href:null,disabled: true);
    private BreadcrumbItem _imageNamePath = new(Image?.Name, $"/images/manage/{Image?.Id}");
    private BreadcrumbItem _profiles = new("Image Profiles", href:null,disabled: true);
    private BreadcrumbItem _profileName = new(Profile?.Name, href:null,disabled: true);

    public List<BreadcrumbItem> Manage()
    {
        return [_home, _manage];
    }
    public List<BreadcrumbItem> General()
    {
        return [_home, _manage, _imageName, new("General", href: null, disabled: true)];
    }
    public List<BreadcrumbItem> Categories()
    {
        return [_home, _manage, _imageName, new("Categories", href: null, disabled: true)];
    }
    public List<BreadcrumbItem> History()
    {
        return [_home, _manage, _imageName, new("History", href: null, disabled: true)];
    }
    
    public List<BreadcrumbItem> ProfileScripts()
    {
        return [_home, _manage, _imageNamePath,_profiles,_profileName, new("Custom Scripts", href: null, disabled: true)];
    }
    public List<BreadcrumbItem> ProfileGeneral()
    {
        return [_home, _manage, _imageNamePath,_profiles,_profileName, new("General", href: null, disabled: true)];
    }
}