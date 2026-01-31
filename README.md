# DotNetNuke.Web.MvcWrapper

A standalone MVC wrapper library that backports MVC Pipeline functionality to DNN 9.x installations.

## Overview

This library provides the core MVC wrapper infrastructure from the DNN Platform 10+ MVC Pipeline, allowing developers to create Razor-based module controls that work in DNN 9.x environments. It enables a smoother migration path for modules that want to adopt MVC patterns before upgrading to MVC Pipeline.

## Features

- **WrapperModule**: WebForms wrapper that hosts MVC module controls inside classic DNN modules
- **RazorModuleControlBase**: Base class for creating Razor-based MVC module controls
- **View Rendering**: Full support for rendering Razor views with models and view data
- **Module Messages**: Built-in helpers for displaying standardized DNN module messages
- **Page Resource Management**: Support for registering CSS/JS resources through IPageContributor

## Requirements

- DNN Platform 9.x (tested with 9.13.2)
- .NET Framework 4.7.2 or higher
- ASP.NET MVC 5.2.x
- ASP.NET Razor 3.2.x

## Installation

### Option 1: Binary Reference (not yet)

1. Download the latest release DLL from the releases page
2. Copy `DotNetNuke.Web.MvcWrapper.dll` to your DNN site's `bin` folder
3. Restart your application pool

The library will automatically register all `IMvcModuleControl` implementations with DNN's dependency injection container through the `Startup` class (if your DNN 9.x version supports `IDnnStartup`).

### Option 2: NuGet Package (not yet)

```powershell
Install-Package DotNetNuke.Web.MvcWrapper
```

### Option 3: Build from Source

1. Clone this repository
2. Open `DotNetNuke.Web.MvcWrapper.sln` in Visual Studio
3. Restore NuGet packages
4. Build the solution
5. Copy the resulting DLL from `src\DotNetNuke.Web.MvcWrapper\bin\Release\` to your DNN site's `bin` folder

## Usage

### Creating a Razor Module Control

1. Create a class that inherits from `RazorModuleControlBase`:

```csharp
using DotNetNuke.Web.MvcPipeline.ModuleControl;
using DotNetNuke.Web.MvcPipeline.ModuleControl.Razor;

namespace MyCompany.Modules.MyModule
{
    public class ViewControl : RazorModuleControlBase
    {
        public override IRazorModuleResult Invoke()
        {
            // Simple content
            return Content("<h1>Hello from MVC!</h1>");
            
            // Or render a view
            var model = new MyViewModel { Title = "My Module" };
            return View(model);
            
            // Or show an error
            return Error("Error", "Something went wrong");
        }
    }
}
```

2. Register the control Wrapper in your module's .dnn manifest:

### Using the WebForms Wrapper

The `WrapperModule` class can be used as a wrapper control in your module:

```xml
<moduleControl>
  <controlKey></controlKey>
  <controlSrc>MyCompany.Modules.MyModule.ViewModuleWrapper, MyModule</controlSrc>
  <controlTitle>View</controlTitle>
  <controlType>View</controlType>
</moduleControl>
```

### Rendering Views

Place your Razor views in a `Views` folder within your module directory:

```
DesktopModules/
  MyModule/
    Views/
      View.cshtml
      Edit.cshtml
      web.config
    MyModuleControl.cs
```

In your control:

```csharp
public override IRazorModuleResult Invoke()
{
    var model = GetMyModel();
    return View(model); // Renders View.cshtml by default
    
    // Or specify a view name
    return View(model);
}
```

### Displaying Module Messages

```csharp
// In your Razor view
@Html.ModuleMessage("Operation completed successfully!", ModuleMessageType.Success)

@Html.ModuleErrorMessage("An error occurred", "Error Details")

@Html.ModuleInfoMessage("This is an informational message")

@Html.ModuleWarningMessage("Warning: Please review your settings")
```

### Contributing Page Resources

Implement `IPageContributor` to register scripts and styles:

```csharp
public class ViewControl : RazorModuleControlBase, IPageContributor
{
    public void ConfigurePage(PageConfigurationContext context)
    {
        // Register scripts
        context.ClientResourceController.RegisterScript(
            "~/DesktopModules/MyModule/js/module.js", 
            true);
        
        // Register stylesheets
        context.ClientResourceController.RegisterStylesheet(
            "~/DesktopModules/MyModule/css/module.css", 
            true);
    }
    
    public override IRazorModuleResult Invoke()
    {
        return View();
    }
}
```

## API Reference

### RazorModuleControlBase

Base class for Razor-based module controls.

**Properties:**
- `ModuleContext`: Access to DNN module context
- `PortalSettings`: Current portal settings
- `UserInfo`: Current user information
- `HttpContext`: Current HTTP context
- `Request`: Current HTTP request
- `ViewData`: View data dictionary

**Methods:**
- `Invoke()`: Abstract method to implement module logic
- `View()`: Renders the default view
- `View(string viewName)`: Renders a specific view
- `View(object model)`: Renders the default view with a model
- `View(string viewName, object model)`: Renders a specific view with a model
- `Content(string content)`: Returns HTML content
- `Error(string heading, string message)`: Displays an error message

### Module Message Types

- `ModuleMessageType.Info`: Informational message (blue)
- `ModuleMessageType.Success`: Success message (green)
- `ModuleMessageType.Warning`: Warning message (yellow/orange)
- `ModuleMessageType.Error`: Error message (red)

## Migration Path to DNN 10+

When you're ready to upgrade to DNN MVC Pipeline:

1. Remove the reference to `DotNetNuke.Web.MvcWrapper`
2. Add a reference to `DotNetNuke.Web.MvcPipeline` (included in DNN 10+)
3. Your code should work without changes due to namespace compatibility
4. Test thoroughly to ensure all functionality works with the native MVC Pipeline

## Troubleshooting

### Views Not Found

Verify:
- Views are in the correct folder: `DesktopModules/[YourModule]/Views/`
- View file names match what you're calling (case-sensitive)
- Views have the `.cshtml` extension
- Views are set to "Copy to Output Directory" if using a separate project

## Contributing

Contributions are welcome! Please submit pull requests or open issues on GitHub.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Credits

This library is based on the DNN Platform MVC Pipeline. It has been adapted and simplified for use with DNN 9.x.

## Support

For issues, questions, or contributions:
- GitHub Issues: [Report issues](https://github.com/yourusername/DotNetNuke.Web.MvcWrapper/issues)
- DNN Community Forums: [https://dnncommunity.org/](https://dnncommunity.org/)

## Dependency Injection Support

The library includes automatic service registration for all `IMvcModuleControl` implementations:

- **Startup.cs**: Implements `IDnnStartup` for automatic configuration
- **StartupExtensions.cs**: Extension methods for registering MVC module controls

If your DNN 9.x installation supports `IDnnStartup`, service registration happens automatically. Otherwise, you can manually register services in your own startup code:

```csharp
using DotNetNuke.Web.MvcPipeline.Extensions;
using Microsoft.Extensions.DependencyInjection;

public void ConfigureServices(IServiceCollection services)
{
    services.AddMvcModuleControls();
}
```

## Version History

### 1.0.0 (2026-01-31)
- Initial release
- Core MVC wrapper functionality
- Razor module control base classes
- View rendering support
- Module message helpers
- Page resource configuration
- Dependency injection integration
- Automatic service registration via Startup class
