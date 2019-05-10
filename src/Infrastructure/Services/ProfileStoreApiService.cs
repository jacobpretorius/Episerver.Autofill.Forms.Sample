using Newtonsoft.Json.Linq;
using RestSharp;

public static class ProfileStoreApiService
{
    // This should come from app settings
    private static readonly string apiRootUrl = "https://somesecret.profilestore.episerver.net"; // get from Insight / Profile store developer portal
    private static readonly string subscriptionKey = "somesecret"; // get from Insight / Profile store developer portal

    public static JToken GetProfileByDeviceId(string deviceId)
    {
        // Set up the request
        var client = new RestClient(apiRootUrl);
        var request = new RestRequest("api/v1.0/Profiles", Method.GET);
        request.AddHeader("Ocp-Apim-Subscription-Key", subscriptionKey);

        // Filter the profiles based on the current device id
        request.AddParameter("$filter", "DeviceIds eq " + deviceId);

        // Execute the request to get the profile
        var getProfileResponse = client.Execute(request);
        var getProfileContent = getProfileResponse.Content;

        // Get the results as a JArray object
        if (!string.IsNullOrWhiteSpace(getProfileContent))
        {
            var profileResponseObject = JObject.Parse(getProfileContent);
            var profileArray = (JArray)profileResponseObject["items"];

            // Expecting an array of profiles with one item in it
            return profileArray.First;
        }

        return null;
    }
}