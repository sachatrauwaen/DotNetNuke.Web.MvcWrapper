// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.MvcPipeline.ModuleControl.WebForms
{
    using System;
    using System.Web.UI;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Modules.Actions;
    using DotNetNuke.Instrumentation;
    using DotNetNuke.UI.Skins;
    using DotNetNuke.UI.Skins.Controls;
    using DotNetNuke.Web.MvcPipeline.ModuleControl.Page;
    using DotNetNuke.Web.MvcPipeline.Utils;

    /// <summary>
    /// WebForms wrapper module that hosts an MVC module control inside a classic DNN module.
    /// </summary>
    public abstract class WrapperModule : PortalModuleBase, IActionable
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(WrapperModule));

        private string html = string.Empty;

        /// <inheritdoc/>
        public ModuleActionCollection ModuleActions { get; private set; } = new ModuleActionCollection();

        /// <summary>
        /// Ensures child controls are created and wired up for the wrapped MVC output.
        /// </summary>
        protected override void CreateChildControls()
        {
            this.Controls.Clear();
            this.Controls.Add(new LiteralControl(this.html));

            // important so ASP.NET tracks the created controls across postbacks
            this.ChildControlsCreated = true;
            base.CreateChildControls();
        }

        /// <summary>
        /// Ensures the wrapped MVC module control is created and rendered early in the page lifecycle.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            try
            {
                var moduleControl = CreateModuleControl(this.ModuleConfiguration);

                if (moduleControl is IActionable actionable)
                {
                    this.ModuleActions = actionable.ModuleActions;
                }

                if (moduleControl is IPageContributor pageContributor)
                {
                    pageContributor.ConfigurePage(new PageConfigurationContext(this.DependencyProvider));
                }

                this.html = MvcViewEngine.RenderHtmlHelperToString(helper => moduleControl.Html(helper));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Skin.AddModuleMessage(this, "An error occurred while loading the module. Please contact the site administrator.", ModuleMessage.ModuleMessageType.RedError);
                this.html = "<div class=\"dnnFormMessage dnnFormError\">" + ex.Message + "</div>";
            }

            this.EnsureChildControls();
        }

        /// <summary>
        /// Crée l'instance du contrôle de module MVC à encapsuler.
        /// </summary>
        /// <param name="moduleConfiguration">La configuration du module à utiliser pour créer le contrôle.</param>
        /// <returns>Une instance de <see cref="IMvcModuleControl"/> représentant le contrôle MVC à encapsuler.</returns>
        protected abstract IMvcModuleControl CreateModuleControl(ModuleInfo moduleConfiguration);
    }
}
