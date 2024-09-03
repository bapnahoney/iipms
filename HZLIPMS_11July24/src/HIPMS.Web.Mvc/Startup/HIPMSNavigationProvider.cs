using Abp.Application.Navigation;
using Abp.Authorization;
using Abp.Localization;
using HIPMS.Authorization;

namespace HIPMS.Web.Startup
{
    /// <summary>
    /// This class defines menus for the application.
    /// </summary>
    public class HIPMSNavigationProvider : NavigationProvider
    {
        public override void SetNavigation(INavigationProviderContext context)
        {
            context.Manager.MainMenu
                .AddItem(
                    new MenuItemDefinition(
                        PageNames.About,
                        L("About"),
                        url: "About",
                        icon: "fas fa-info-circle"
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        PageNames.Home,
                        L("HomePage"),
                        url: "",
                        icon: "fas fa-home",
                        requiresAuthentication: true
                    )
                )
                //.AddItem(
                //    new MenuItemDefinition(
                //        PageNames.Tenants,
                //        L("Tenants"),
                //        url: "Tenants",
                //        icon: "fas fa-building",
                //        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Tenants)
                //    )
                //)
                .AddItem(
                    new MenuItemDefinition(
                        PageNames.IC,
                        L("ICShort"),
                        url: "",
                        icon: "fas fa-user-clock",
                        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_IC)
                    )
                    .AddItem(
                        new MenuItemDefinition(
                        PageNames.ViewIC,
                        L("ViewIC"),
                        url: "IC",
                        icon: "fas fa-circle",
                        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_IC)
                    )
                  )
                    .AddItem(
                        new MenuItemDefinition(
                        PageNames.CreateIC,
                        L("CreateIC"),
                        url: "IC/Create",
                        icon: "fas fa-circle",
                        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_IC)
                    )
                  )
                )
                .AddItem(
                    new MenuItemDefinition(
                        PageNames.RFI,
                        L("RFIShort"),
                        url: "",
                        icon: "fas fa-user-times",
                        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_IC)
                    )
                    .AddItem(
                        new MenuItemDefinition(
                        PageNames.ViewRFI,
                        L("ViewRFI"),
                        url: "RFI",
                        icon: "fas fa-circle",
                        customData: "test",
                        order: 1,
                        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_IC)
                    )
                  )
                    .AddItem(
                        new MenuItemDefinition(
                        PageNames.CreateRFI,
                        L("CreateRFI"),
                        url: "RFI/Create",
                        icon: "fas fa-circle",
                        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_IC)
                    )
                  )
                )
                .AddItem(
                    new MenuItemDefinition(
                        PageNames.NCR,
                        L("NCR"),
                        url: "",
                        icon: "fas fa-venus",
                        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_IC)
                    )
                    .AddItem(
                        new MenuItemDefinition(
                        PageNames.ViewNCR,
                        L("ViewNCR"),
                        url: "NCR",
                        icon: "fas fa-circle",
                        customData: "test",
                        order: 1,
                        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_IC)
                    )
                  )
                    .AddItem(
                        new MenuItemDefinition(
                        PageNames.CreateNCR,
                        L("CreateNCR"),
                        url: "NCR/Create",
                        icon: "fas fa-circle",
                        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_IC)
                    )
                  )
                )
                .AddItem(
                    new MenuItemDefinition(
                        PageNames.POMap,
                        L("POUserMap"),
                        url: "",
                        icon: "fas fa-venus",
                        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Admin)
                    )
                    .AddItem(
                        new MenuItemDefinition(
                        PageNames.ViewUserPOMap,
                        L("ViewUserPOMap"),
                        url: "PO/View",
                        icon: "fas fa-circle",
                        customData: "test",
                        order: 1,
                        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Admin)
                    )
                  )
                    .AddItem(
                        new MenuItemDefinition(
                        PageNames.CreateUserPOMap,
                        L("CreateUserPOMap"),
                        url: "PO/POUserDD",
                        icon: "fas fa-circle",
                        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Admin)
                    )
                  )
                )

                //.AddItem( // Menu items below is just for demonstration!
                //    new MenuItemDefinition(
                //        "Services",
                //        L("Services"),
                //        icon: "fas fa-circle"
                //    ).AddItem(
                //        new MenuItemDefinition(
                //            "NCR",
                //            new FixedLocalizableString("NCR"),
                //            icon: "far fa-circle"
                //        ).AddItem(
                //            new MenuItemDefinition(
                //                "View NCR",
                //                new FixedLocalizableString("View"),
                //                url: "NCR",
                //                icon: "far fa-dot-circle"
                //            )
                //        ).AddItem(
                //            new MenuItemDefinition(
                //                "NCRCreate",
                //                new FixedLocalizableString("NCRCreate"),
                //                url: "NCR/Create",
                //                icon: "far fa-dot-circle"
                //            )
                //        )
                //    ).AddItem(
                //        new MenuItemDefinition(
                //            "RFI",
                //            new FixedLocalizableString("RFI"),
                //            icon: "far fa-circle"
                //        ).AddItem(
                //            new MenuItemDefinition(
                //                "RFI",
                //                new FixedLocalizableString("RFIView"),
                //                url: "RFI",
                //                icon: "far fa-dot-circle"
                //            )
                //        ).AddItem(
                //            new MenuItemDefinition(
                //                "RFICreate",
                //                new FixedLocalizableString("RFICreate"),
                //                url: "RFI/Create",
                //                icon: "far fa-dot-circle"
                //            )
                //        )
                //    )
                //)

                .AddItem(
                    new MenuItemDefinition(
                        PageNames.Users,
                        L("Users"),
                        url: "Users",
                        icon: "fas fa-users",
                        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Users)
                    )
                ).AddItem(
                    new MenuItemDefinition(
                        PageNames.Roles,
                        L("Roles"),
                        url: "Roles",
                        icon: "fas fa-theater-masks",
                        permissionDependency: new SimplePermissionDependency(PermissionNames.Pages_Roles)
                    )
                )

                //.AddItem( // Menu items below is just for demonstration!
                //    new MenuItemDefinition(
                //        "MultiLevelMenu",
                //        L("MultiLevelMenu"),
                //        icon: "fas fa-circle"
                //    ).AddItem(
                //        new MenuItemDefinition(
                //            "AspNetBoilerplate",
                //            new FixedLocalizableString("ASP.NET Boilerplate"),
                //            icon: "far fa-circle"
                //        ).AddItem(
                //            new MenuItemDefinition(
                //                "AspNetBoilerplateHome",
                //                new FixedLocalizableString("Home"),
                //                url: "https://aspnetboilerplate.com?ref=abptmpl",
                //                icon: "far fa-dot-circle"
                //            )
                //        ).AddItem(
                //            new MenuItemDefinition(
                //                "AspNetBoilerplateTemplates",
                //                new FixedLocalizableString("Templates"),
                //                url: "https://aspnetboilerplate.com/Templates?ref=abptmpl",
                //                icon: "far fa-dot-circle"
                //            )
                //        ).AddItem(
                //            new MenuItemDefinition(
                //                "AspNetBoilerplateSamples",
                //                new FixedLocalizableString("Samples"),
                //                url: "https://aspnetboilerplate.com/Samples?ref=abptmpl",
                //                icon: "far fa-dot-circle"
                //            )
                //        ).AddItem(
                //            new MenuItemDefinition(
                //                "AspNetBoilerplateDocuments",
                //                new FixedLocalizableString("Documents"),
                //                url: "https://aspnetboilerplate.com/Pages/Documents?ref=abptmpl",
                //                icon: "far fa-dot-circle"
                //            )
                //        )
                //    ).AddItem(
                //        new MenuItemDefinition(
                //            "AspNetZero",
                //            new FixedLocalizableString("ASP.NET Zero"),
                //            icon: "far fa-circle"
                //        ).AddItem(
                //            new MenuItemDefinition(
                //                "AspNetZeroHome",
                //                new FixedLocalizableString("Home"),
                //                url: "https://aspnetzero.com?ref=abptmpl",
                //                icon: "far fa-dot-circle"
                //            )
                //        ).AddItem(
                //            new MenuItemDefinition(
                //                "AspNetZeroFeatures",
                //                new FixedLocalizableString("Features"),
                //                url: "https://aspnetzero.com/Features?ref=abptmpl",
                //                icon: "far fa-dot-circle"
                //            )
                //        ).AddItem(
                //            new MenuItemDefinition(
                //                "AspNetZeroPricing",
                //                new FixedLocalizableString("Pricing"),
                //                url: "https://aspnetzero.com/Pricing?ref=abptmpl#pricing",
                //                icon: "far fa-dot-circle"
                //            )
                //        ).AddItem(
                //            new MenuItemDefinition(
                //                "AspNetZeroFaq",
                //                new FixedLocalizableString("Faq"),
                //                url: "https://aspnetzero.com/Faq?ref=abptmpl",
                //                icon: "far fa-dot-circle"
                //            )
                //        ).AddItem(
                //            new MenuItemDefinition(
                //                "AspNetZeroDocuments",
                //                new FixedLocalizableString("Documents"),
                //                url: "https://aspnetzero.com/Documents?ref=abptmpl",
                //                icon: "far fa-dot-circle"
                //            )
                //        )
                //    )
                //)
                ;
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, HIPMSConsts.LocalizationSourceName);
        }
    }
}