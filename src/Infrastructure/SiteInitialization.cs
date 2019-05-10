using EPiServer.Forms.Core.Internal.Autofill;
using EPiServer.Forms.Core.Internal.ExternalSystem;
using EPiServer.Framework;
using EPiServer.Reference.Commerce.Site.Infrastructure.Autofill;
using EPiServer.ServiceLocation;

namespace EPiServer.Reference.Commerce.Site.Infrastructure
{
	[ModuleDependency(typeof(EPiServer.Commerce.Initialization.InitializationModule))]
	public class SiteInitialization : IConfigurableModule
	{
		public void ConfigureContainer(ServiceConfigurationContext context)
		{
			var services = context.Services;

			// Register our autofill service
			services.AddSingleton<IAutofillProvider, ProfileStoreAutofillFields>();
			services.AddSingleton<IExternalSystem, ProfileStoreAutofillFields>();
		}
	}
}