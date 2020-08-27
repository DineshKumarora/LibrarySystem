using System.Text;
using System.Net.Http.Headers;

namespace ClientApp
{
    using System;
    using System.Net.Http;
    using Newtonsoft.Json;
    
    class Program
    {
        private static string _uName = string.Empty;
        private static string _uPassword = string.Empty;
        
        private static string RegisterUserJson(string bookId, string subscriberName, string email)
        {
            string registerUserJson = "{"
                                      //+ "Subscription\": {"
                                      + "\"BookId\": \"" + bookId + "\","
                                      + "\"SubscriberName\": \"" + subscriberName
                                      + "\"," + "\"Email\": \"" + email
                                      + "\"" + "}"; 
                                      //+ "}";
            return registerUserJson;
        }

        private static string RegisterSubscriptionJson(string bookId, string subscriberName, bool notify)
        {
            string registerSubscriptionJson = "{"
                                              + "\"BookId\": \"" + bookId + "\","
                                              + "\"SubscriberName\": \"" + subscriberName
                                              + "\"," + "\"Notify\": \"" + notify
                                              + "\"" + "}";
            return registerSubscriptionJson;
        }

        

        private static void GetBooks(HttpClient client, bool allRecords = true)
        {
            HttpResponseMessage resWithToken;
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {GetJwt()}");

            if (!allRecords)
            {
                Console.WriteLine("Enter Book Id :");
                var bookId = Console.ReadLine();
                resWithToken = client.GetAsync($"/Books/{bookId}").Result;
                Console.WriteLine("\nRetrieve a particular book record, Books API.");
            }
            else
            {
                resWithToken = client.GetAsync("/Books").Result;
                Console.WriteLine("\nRetrieve all books details, Books API.");
            }

            Console.WriteLine($"Result : {resWithToken.StatusCode}");
            Console.WriteLine(resWithToken.Content.ReadAsStringAsync().Result);
        }

        private static void SubscribeBooks(HttpClient client)
        {
            Console.WriteLine("Enter Book Id :");
            string bookId = Console.ReadLine();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {GetJwt()}");

            StringContent content = new StringContent(bookId, Encoding.UTF8, "application/json");
            var resWithToken = client.PutAsync($"/Books/{bookId}", content).Result;

            Console.WriteLine("\nBook Subscription Books API , with token.");
            Console.WriteLine($"Result : {resWithToken.StatusCode}");
            Console.WriteLine(resWithToken.Content.ReadAsStringAsync().Result);
        }

        private static void SubscribeList(HttpClient client, bool allSubscription)
        {
            HttpResponseMessage resWithToken;
            //Get the Books Subscription list and return result.
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {GetJwt()}");
            if (allSubscription)
            {
                resWithToken = client.GetAsync("/Subscription").Result;
                Console.WriteLine("\nBook Subscription List, Subscription API.");
            }
            else
            {
                Console.WriteLine("Enter Subscriber Name:");
                var subscriberId = Console.ReadLine();
                resWithToken = client.GetAsync($"/Subscription/{subscriberId}").Result;
                Console.WriteLine("\nRetrieve a particular member subscription list, Subscription API.");
            }
            Console.WriteLine($"Result : {resWithToken.StatusCode}");
            Console.WriteLine(resWithToken.Content.ReadAsStringAsync().Result);
        }

        private static void PostNewSubscription(HttpClient client)
        {
            Console.WriteLine("Enter Book Id :");
            string bookId = Console.ReadLine();
            Console.WriteLine("Enter Subscriber Name :");
            string name = Console.ReadLine();
            Console.WriteLine("Notify me whenever book is available (Y or N):");
            string notify = Console.ReadLine();

            string registerSubscriptionJson = RegisterSubscriptionJson(bookId, name, (notify == "Y"));

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {GetJwt()}");
            HttpRequestMessage request = new HttpRequestMessage
            {
                Content = new StringContent(registerSubscriptionJson, Encoding.UTF8,
                    "application/json")
            };

            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage resWithToken = client.PostAsync("/Subscription", request.Content).Result;

            Console.WriteLine("\nBook Subscription Subscription API.");
            Console.WriteLine($"Result : {resWithToken.StatusCode}");
            Console.WriteLine(resWithToken.Content.ReadAsStringAsync().Result);
        }

        private static void SendAlerts(HttpClient client)
        {
            Console.WriteLine("Enter Book Id :");
            string bookId = Console.ReadLine();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {GetJwt()}");

            var resWithToken = client.GetAsync($"/AlertSubscription/{bookId}").Result;
            Console.WriteLine("\nSend Alert in case of books available, AlertSubscription API.");

            Console.WriteLine("Result : {resWithToken.StatusCode}");
            Console.WriteLine(resWithToken.Content.ReadAsStringAsync().Result);
        }

        private static void AlertSubscription(HttpClient client)
        {
            //Add the alert for the Subscriber as user opted for Notification of availability
            Console.WriteLine("Enter Book Id :");
            string bookId = Console.ReadLine();
            Console.WriteLine("Enter User Id :");
            string userId = Console.ReadLine();
            Console.WriteLine("Enter Email Id :");
            string emailId = Console.ReadLine();

            string registerUserJson = RegisterUserJson(bookId, userId, emailId);

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {GetJwt()}");
            HttpRequestMessage request = new HttpRequestMessage
            {
                Content = new StringContent(registerUserJson, Encoding.UTF8,
                    "application/json")
            };

            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var resWithToken = client.PostAsync("/AlertSubscription", request.Content).Result;

            Console.WriteLine("\nAlert Subscription for Book availability, Alert Subscription API.");
            Console.WriteLine($"Result : {resWithToken.StatusCode}");
            Console.WriteLine(resWithToken.Content.ReadAsStringAsync().Result);
        }

        static void Main(string[] args)
        {
            HttpClient client = new HttpClient {BaseAddress = new Uri("http://localhost:62793")};

            Console.WriteLine("Enter User Name :");
            _uName = Console.ReadLine();

            Console.WriteLine("Enter Password Name :");
            _uPassword = Console.ReadLine();

            //With access_token will access the service and return book list result.
            GetBooks(client);

            //Retrieve Particular books details and return result.
            GetBooks(client, false);

            //Update Availability or Subscribe a book
            SubscribeBooks(client);

            //Get the Books Subscription list and return result.
            SubscribeList(client, true);

            //Get the Books Subscription list of specific Member and return result.
            SubscribeList(client, false);

            //Not Tested
            //Post New Subscription and if book not available add alert and send alert of more books available
            PostNewSubscription(client);

            //Alert Subscription
            AlertSubscription(client);

            SendAlerts(client);

            Console.Read();
        }

        private static dynamic _jwt;
        private static string GetJwt()
        {
            HttpClient client = new HttpClient {BaseAddress = new Uri("http://localhost:62793")};

            client.DefaultRequestHeaders.Clear();
            var res2 = client.GetAsync($"/api/auth?name={_uName}&pwd={_uPassword}").Result;

            _jwt = JsonConvert.DeserializeObject(res2.Content.ReadAsStringAsync().Result);

            Console.WriteLine($"\nToken={_jwt.access_token}");

            return _jwt.access_token;
        }
    }
}
