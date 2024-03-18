using ConsoleApp2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

class TestDemo
{
    public static string token = string.Empty;
    public static List<string> categories = new List<string>();
    public static List<string> magazinesByCategory = new List<string>();
    public static List<string> subscribers = new List<string>();
    public static List<string> subscriberIds = new List<string>();

    // Main Method 
    public static void Main(String[] args)
    {
        token = GetToken();
        categories = GetCategories(token);
        magazinesByCategory = GetListOfMagazinesForEachCategory(token,categories);
        subscribers = GetListOfSubscribers(token);
        foreach (var id in subscribers.ToArray())
        {
            dynamic data = JObject.Parse(id);
            Console.WriteLine("SubscribersIds subscribed to at least one magazine in each category: " + data.id + " firstname: " + data.firstName + " MagazineIds: " + data.magazineIds);
        }
        Console.ReadLine();

    }

    private static HttpClient GetHttpClientDetails()
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri("http://magazinestore.azurewebsites.net");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
        return client;
    }

    private static List<string> GetListOfSubscribers(string token)
    {
        var client = GetHttpClientDetails();
        string path = $"/api/subscribers/";
        HttpResponseMessage response = client.GetAsync(path + token).Result;
        response.EnsureSuccessStatusCode();
        var responseBody = response.Content.ReadAsStringAsync().Result;
        var res = (JObject)JsonConvert.DeserializeObject(responseBody.ToString());
        var subscribers = new List<string>();
        foreach (var item in res["data"].ToArray())
        {
            subscribers.Add(item.ToString());
        }
        return subscribers;
    }

    private static List<string> GetListOfMagazinesForEachCategory(string token,List<string> categories)
    {
        var magazines = new List<string>();
        foreach (var category in categories)
        {
            var client = GetHttpClientDetails();
            string path = $"/api/magazines/";
            HttpResponseMessage response = client.GetAsync(path + token + "/" + category).Result;
            response.EnsureSuccessStatusCode();
            var responseBody = response.Content.ReadAsStringAsync().Result;
            var res = (JObject)JsonConvert.DeserializeObject(responseBody.ToString());
            foreach (var item in res["data"].ToArray())
            {
                magazines.Add(item.ToString());
            }
        }
        return magazines;
    }

    private static List<string> GetCategories(string token)
    {
        var client = GetHttpClientDetails();
        string path = $"/api/categories/";
        HttpResponseMessage response = client.GetAsync(path + token).Result;
        response.EnsureSuccessStatusCode();
        var responseBody = response.Content.ReadAsStringAsync().Result;
        var res = (JObject)JsonConvert.DeserializeObject(responseBody.ToString());
        var categories = new List<string>();
        foreach (var item in res["data"].ToArray())
        {
            categories.Add(item.ToString());
        }
        return categories;
    }

    private static string GetToken()
    {
        // Call asynchronous network methods in a try/catch block to handle exceptions.
        try
        {
            var client = GetHttpClientDetails();
            string path = $"/api/token";
            HttpResponseMessage response = client.GetAsync(path).Result;
            response.EnsureSuccessStatusCode();
            var responseBody = response.Content.ReadAsStringAsync().Result;
            var res = (JObject)JsonConvert.DeserializeObject(responseBody.ToString());
            return res["token"].Value<string>();
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
        }
        return string.Empty;
    }
}
