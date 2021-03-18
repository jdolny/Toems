using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToemsUI.Client.Services
{
    public class NavItem
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Icon { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Expanded { get; set; }

        public bool RequiresId { get; set; }
        public IEnumerable<NavItem> Children { get; set; }
        public IEnumerable<string> Tags { get; set; }

    }
    public class DefaultNavigationService
    {
        NavItem[] navItems = new[]
        {
            new NavItem()
            {
                Name = "Dashboard",
                Path = "/",
                Icon = "&#xe90e"
            },
            new NavItem()
            {
                Name = "Modules",
                Path = "",
                Icon = "&#xe901",
                Children = new []
                {
                    new NavItem()
                    {
                        Name = "Command Modules",
                        Children = new []
                        {
                            new NavItem()
                            {
                                Name = "View Command Modules",
                                Path = "/Modules/Command"

                            },
                            new NavItem()
                            {
                                Name = "New Command Module",
                                Path = "/Modules/Command/Create"
                            },
                            new NavItem()
                            {
                                Name = "Details",
                                RequiresId = true,
                                Path = "/Modules/Command/Details"
                            },
                            new NavItem()
                            {
                                Name = "Files",
                                RequiresId = true,
                                Path = "/Modules/Command/Files"
                            },
                            new NavItem()
                            {
                                Name = "Usages",
                                RequiresId = true
                            },
                            new NavItem()
                            {
                                Name = "Categories",
                                RequiresId = true
                            },
                            new NavItem()
                            {
                                Name = "External Files",
                                RequiresId = true
                            }


                        }
                    },
                    new NavItem()
                    {
                        Name = "File Copy Modules",
                    },
                    new NavItem()
                    {
                        Name = "Message Modules"
                    },
                    new NavItem()
                    {
                        Name = "Printer Modules"
                    },
                    new NavItem()
                    {
                        Name = "Script Modules"
                    },
                    new NavItem()
                    {
                        Name = "Software Modules"
                    },
                    new NavItem()
                    {
                        Name = "Sysprep Modules"
                    },
                    new NavItem()
                    {
                        Name = "Windows Update Modules"
                    }

                }
            },
            new NavItem()
            {
                Name = "Policies",
                Path = "/Policies",
                Icon = "&#xe905"
            },
            new NavItem()
            {
                Name = "Groups",
                Path = "/Groups",
                Icon = "&#xe900"
            },
            new NavItem()
            {
                Name = "Computers",
                Path = "",
                Icon = "&#xe907",
                Children = new []
                {
                    new NavItem()
                    {
                        Name = "Active",
                        Path = "/Computers"
                    },
                    new NavItem()
                    {
                        Name = "Image Only",
                        Path = "Computers/ImageOnly"
                    },
                    new NavItem()
                    {
                        Name = "All",
                        Path = "Computers/All"
                    },
                    new NavItem()
                    {
                        Name = "Pre-Provision",
                        Path = "Computers/Preprovision"
                    },
                    new NavItem()
                    {
                        Name = "Approval Requests",
                        Path = "Computers/Approval"
                    },
                    new NavItem()
                    {
                        Name = "Reset Requests",
                        Path = "Computers/Reset"
                    },
                    new NavItem()
                    {
                        Name = "Inventory",
                        RequiresId = true
                    },
                    new NavItem()
                    {
                        Name = "Windows Updates",
                        RequiresId = true
                    }
                }
            },
            new NavItem()
            {
                Name = "Images",
                Path = "/Images",
                Icon = "&#xe009"
            },
            new NavItem()
            {
                Name = "Imaging Tasks",
                Path = "/ImagingTasks",
                Icon = "&#xe020"
            },
            new NavItem()
            {
                Name = "Global Properties",
                Path = "/GlobalProperties",
                Icon = "&#xe910"
            },
            new NavItem()
            {
                Name = "Reports",
                Path = "/Reports",
                Icon = "&#xe90d"
            },
            new NavItem()
            {
                Name = "Users",
                Path = "/Users",
                Icon = "&#xe909"
            },
            new NavItem()
            {
                Name = "Admin Settings",
                Path = "/AdminSettings",
                Icon = "&#xe90b"
            },
        };
        public IEnumerable<NavItem> NavItems
        {
            get
            {
                return navItems;
            }
        }

        public IEnumerable<NavItem> Filter(string term)
        {
            if (string.IsNullOrEmpty(term))
                return NavItems;

            //return NavItems.Where(x => x.Name.Contains(term, StringComparison.OrdinalIgnoreCase));

            Func<string, bool> contains = value => value.Contains(term, StringComparison.OrdinalIgnoreCase);

            Func<NavItem, bool> filter = (navItem) => contains(navItem.Name) || (navItem.Tags != null && navItem.Tags.Any(contains));

            return NavItems.Where(category => category.Children != null && category.Children.Any(filter))
                           .Select(category => new NavItem()
                           {
                               Name = category.Name,
                               Expanded = true,
                               Children = category.Children.Where(filter).ToArray()
                           }).ToList();
        }
    }
}
