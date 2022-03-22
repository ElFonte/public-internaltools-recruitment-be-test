using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SuperPanel.App.Data;
using SuperPanel.App.Infrastructure;
using SuperPanel.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace SuperPanel.Tests
{
    public class UserRepositoryTests
    {
        static List<Contact> _contacts = new List<Contact>();
        public static HttpClient ApiClient { get; set; }

        public static void Main(string[] args)
        {
            RunTest().Wait();
        }


        private static async Task RunTest()
        {
            //Test procedure: We'll get all the contacts, find the first non-anonimized contact and anonimize it, then get the contact again and check it's status
            InitializeHTTPClient();
            GetContacts().Wait();
            long id = -1;
            foreach (Contact contact in _contacts)
            {
                if (!contact.IsAnonymized)
                {
                    id = contact.Id;
                    break;
                }
            }
            if (id==-1)
                Console.WriteLine("No identifiable users found");
            else
            {
                GDPRContact(id).Wait();
                Contact contact = await GetContactById(id);
                if (contact.IsAnonymized)
                    Console.WriteLine("Operation successfull");
                else
                    Console.WriteLine("Operation failed");
            }
        }

        [Fact]
        private void QueryAll_ShouldReturnEverything()
        {
            var r = new UserRepository(Options.Create<DataOptions>(new DataOptions()
            {
                JsonFilePath = "./../../../../data/users.json"
            }));

            var all = r.QueryAll();

            Assert.Equal(5000, all.Count());
        }
        
        private static async Task GetContacts()
        {
            //Let's get the full user list
            HttpResponseMessage response = await ApiClient.GetAsync("v1/contacts");
            if (response.IsSuccessStatusCode)
            {
                //Success! we have the contact data in JSon format
                //_contacts = JsonSerializer.Deserialize<IEnumerable<Contact>>(await response.Content.ReadAsStringAsync()).ToList();
                _contacts = JsonConvert.DeserializeObject<List<Contact>>(await response.Content.ReadAsStringAsync());
            }
            else
            {
                Console.WriteLine("Internal server Error");
            }
    }
        private static async Task<Contact> GetContactById(long Id)
        {
            //Let's get the user
            Contact contact = new Contact();
            HttpResponseMessage response = await ApiClient.GetAsync("v1/contacts/" + Id.ToString());
            if (response.IsSuccessStatusCode)
            {
                //Success! we have the contact data in JSon format
                contact = JsonConvert.DeserializeObject<Contact>(await response.Content.ReadAsStringAsync());
            }
            else
            {
                Console.WriteLine("Internal server Error");
            }
            return contact;
        }
        static async Task GDPRContact(long Id)
        {
            //Let's get the user
            HttpResponseMessage response = await ApiClient.PutAsync("v1/contacts/" + Id.ToString() + "/gdpr", null);
            //response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                //Success!
            }
            else
            {
                Console.WriteLine("Internal server Error");
            }

        }
        [Fact]
        public static void InitializeHTTPClient()
        {
            ApiClient = new HttpClient();
            ApiClient.BaseAddress = new Uri("http://localhost:61695");
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
    public class Contact
    {
        public long Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool IsAnonymized { get; set; }


        public Contact(int id)
        {
            this.Id = id;
            this.IsAnonymized = false;
        }
        public Contact()
        {
        }
    }
}
