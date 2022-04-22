using System.Net.Http.Headers;
using System.Text;
using RabbitMQ.Client;

namespace DeveloperTools.RabbitMQHelper;

// ref: https://www.rabbitmq.com/management.html
// ref: https://rawcdn.githack.com/rabbitmq/rabbitmq-server/v3.9.14/deps/rabbitmq_management/priv/www/api/index.html
// ref: https://stackoverflow.com/a/45923493

public static class Helper
{
    private const string HostName = "localhost";
    private const string Port = "15672";
    private const string UserName = "guest";
    private const string Password = "guest";
    private const string VirtualHost = "/";
    //private const string VirtualHost = "/xbank";
    private const string BaseAddress = $"http://{HostName}:{Port}/api/";
    private const string AcceptHeader = "application/json";
    
    private const string DefinitionsUrl = $"{BaseAddress}definitions{VirtualHost}";
    
    internal static HttpClient GetClient(bool? authenticated = true)
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri(BaseAddress);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(AcceptHeader));
        if (authenticated.Value)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"{UserName}:{Password}")));
        }
        return client;
    }
    internal static async Task<T> HttpRequestSendAsync<T>(HttpMethod method, string url)
    {
        using var client = GetClient(true);
        using var httpRequestMessage = new HttpRequestMessage(method, url);

        var response = await client.SendAsync(httpRequestMessage);
        response.EnsureSuccessStatusCode();
    
        var jsonString = await response.Content.ReadAsStringAsync();
    
        //return await response.Content.ReadFromJsonAsync<T>();
        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonString);
    }
    
    internal static async Task<List<string>> GetAllQueueNames()
    {
        var getDefinitions = await HttpRequestSendAsync<Nodes>(HttpMethod.Get, DefinitionsUrl);
        return getDefinitions.Queues.Select(x => x.Name).ToList();
    }
    
    internal static async Task PurgeAllQueue(bool? exceptErrorQueue = false)
    {
        var queueNames = await GetAllQueueNames();
        var factory = new ConnectionFactory
        {
            VirtualHost = VirtualHost,
            HostName = HostName,
            UserName = UserName,
            Password = Password
        };

        using (var connection = factory.CreateConnection())
        {
            using (var channel = connection.CreateModel())
            {
                for (var i = 0; i < queueNames.Count; i++)
                {
                    if(exceptErrorQueue.Value && queueNames[i].Contains("error"))
                        continue;
                    
                    channel.QueuePurge(queueNames[i]);
                    Console.WriteLine($"{queueNames[i]} is purged");
                }
            }
        }
    }
    
    
    /*
    internal static void GetAllQueues()
    {
        using (var httpClient = new HttpClient())
        {
            using (var request = new HttpRequestMessage(new HttpMethod("GET"), "http://localhost:15672/api/queues/"))
            {
                var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"guest:guest"));
                request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");

                var response = httpClient.SendAsync(request).Result;

                if (response.IsSuccessStatusCode == true)
                {
                    var jsonContent = response.Content.ReadAsStringAsync().Result;
                    if (!string.IsNullOrEmpty(jsonContent))
                    {
                        Console.WriteLine(jsonContent.ToString());
                    }
                }
            }
        }
    }
    */
    
}