using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer.Forms.Core;
using EPiServer.Forms.Core.Internal.Autofill;
using EPiServer.Forms.Core.Internal.ExternalSystem;
using EPiServer.Reference.Commerce.Site.Infrastructure.Services;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Autofill
{
	public class ProfileStoreAutofillFields : IExternalSystem, IAutofillProvider
	{
		public virtual string Id
		{
			get { return "ProfileStoreAutofillFields"; }
		}

		public virtual IEnumerable<IDatasource> Datasources
		{
			get
			{
				// Register the Profile Store as a data source
				var profileStoreDataSource = new Datasource()
				{
					Id = "ProfileStoreDataSource",
					Name = "Profile Store Data Source",
					OwnerSystem = this,
					Columns = new Dictionary<string, string> {
						// "Name of mapped field", "friendly name in CMS"
						{ "profilestoreemail", "Email" },
						{ "profilestorename", "Name" },
						{ "profilestorecity", "City" },
						{ "profilestoremobile", "Mobile" },
						{ "profilestorephone", "Phone" }
					}
				};

				return new[] { profileStoreDataSource };
			}
		}

		/// <summary>
		/// Returns a list of suggested values by field mapping key. This will be called automatically by the GetAutofillValues() function in DataElementBlockBase for each field
		/// </summary>
		/// <returns>Collection of suggested values</returns>
		public virtual IEnumerable<string> GetSuggestedValues(IDatasource selectedDatasource, IEnumerable<RemoteFieldInfo> remoteFieldInfos, ElementBlockBase content, IFormContainerBlock formContainerBlock, HttpContextBase context)
		{
			if (selectedDatasource == null || remoteFieldInfos == null)
				return Enumerable.Empty<string>();

			// Make sure the Data sources are for this system
			if (!this.Datasources.Any(ds => ds.Id == selectedDatasource.Id)
				|| !remoteFieldInfos.Any(mi => mi.DatasourceId == selectedDatasource.Id))
			{
				return Enumerable.Empty<string>();
			}

			// We also need to make sure that we have some tracking info to auto fill
			// _madid is the default Episerver Profile Store tracking cookie, see https://world.episerver.com/documentation/developer-guides/tracking/episerver-cookies/
			var userDeviceId = context.Request.Cookies["_madid"]?.Value;

			// Because this gets called with EVERY FIELD it is suggested to cache the response elsewhere
			var userProfile = ProfileStoreApiService.GetProfileByDeviceId(userDeviceId);
			if (userProfile == null)
			{
				return Enumerable.Empty<string>();
			}

			// Unpack the info object
			var info = userProfile["Info"];

			// Get the field details
			var activeRemoteFieldInfo = remoteFieldInfos.FirstOrDefault(mi => mi.DatasourceId == selectedDatasource.Id);
			switch (activeRemoteFieldInfo.ColumnId)
			{
				// Suggest the data from the Profile Store user profile
				case "profilestoreemail":
					return new List<string> {
						(string)(info["Email"] ?? info["Email"]?.ToString())
					};

				case "profilestorename":
					return new List<string> {
						(string)(userProfile["Name"] ?? userProfile["Name"]?.ToString())
					};

				case "profilestorecity":
					return new List<string>{
						(string)(info["City"] ?? info["City"]?.ToString())
					};

				case "profilestorephone":
					return new List<string>{
						(string)(info["Phone"] ?? info["Phone"]?.ToString())
					};

				case "profilestoremobile":
					return new List<string>{
						(string)(info["Mobile"] ?? info["Mobile"]?.ToString())
					};

				default:
					return Enumerable.Empty<string>();
			}
		}
	}
}