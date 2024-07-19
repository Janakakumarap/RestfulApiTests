using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;

namespace RestfulApiTests
{
    public class RestfulApiTests
    {
        private readonly HttpClient _client;

        public RestfulApiTests()
        {
            _client = new HttpClient { BaseAddress = new Uri("https://restful-api.dev/") };
        }

        [Fact]
        public async Task Test_GetAllObjects()
        {
            var response = await _client.GetAsync("objects");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseString));
        }

        [Fact]
        public async Task Test_AddObject()
        {
            var newObject = new { Name = "Test Object", Description = "This is a test object" };
            var content = new StringContent(JsonConvert.SerializeObject(newObject), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("objects", content);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var createdObject = JsonConvert.DeserializeObject<dynamic>(responseString);
            Assert.NotNull(createdObject.id);
        }

        [Fact]
        public async Task Test_GetSingleObject()
        {
            var newObject = new { Name = "Test Object", Description = "This is a test object" };
            var content = new StringContent(JsonConvert.SerializeObject(newObject), Encoding.UTF8, "application/json");
            var postResponse = await _client.PostAsync("objects", content);
            postResponse.EnsureSuccessStatusCode();
            var postResponseString = await postResponse.Content.ReadAsStringAsync();
            var createdObject = JsonConvert.DeserializeObject<dynamic>(postResponseString);
            string objectId = createdObject.id;

            var getResponse = await _client.GetAsync($"objects/{objectId}");
            getResponse.EnsureSuccessStatusCode();
            var getResponseString = await getResponse.Content.ReadAsStringAsync();
            var retrievedObject = JsonConvert.DeserializeObject<dynamic>(getResponseString);
            Assert.Equal(objectId, retrievedObject.id.ToString());
        }

        [Fact]
        public async Task Test_UpdateObject()
        {
            var newObject = new { Name = "Test Object", Description = "This is a test object" };
            var content = new StringContent(JsonConvert.SerializeObject(newObject), Encoding.UTF8, "application/json");
            var postResponse = await _client.PostAsync("objects", content);
            postResponse.EnsureSuccessStatusCode();
            var postResponseString = await postResponse.Content.ReadAsStringAsync();
            var createdObject = JsonConvert.DeserializeObject<dynamic>(postResponseString);
            string objectId = createdObject.id;

            var updatedObject = new { Name = "Updated Test Object", Description = "This is an updated test object" };
            var updateContent = new StringContent(JsonConvert.SerializeObject(updatedObject), Encoding.UTF8, "application/json");
            var putResponse = await _client.PutAsync($"objects/{objectId}", updateContent);
            putResponse.EnsureSuccessStatusCode();
            var putResponseString = await putResponse.Content.ReadAsStringAsync();
            var updatedResponseObject = JsonConvert.DeserializeObject<dynamic>(putResponseString);
            Assert.Equal("Updated Test Object", updatedResponseObject.name.ToString());
        }

        [Fact]
        public async Task Test_DeleteObject()
        {
            var newObject = new { Name = "Test Object", Description = "This is a test object" };
            var content = new StringContent(JsonConvert.SerializeObject(newObject), Encoding.UTF8, "application/json");
            var postResponse = await _client.PostAsync("objects", content);
            postResponse.EnsureSuccessStatusCode();
            var postResponseString = await postResponse.Content.ReadAsStringAsync();
            var createdObject = JsonConvert.DeserializeObject<dynamic>(postResponseString);
            string objectId = createdObject.id;

            var deleteResponse = await _client.DeleteAsync($"objects/{objectId}");
            deleteResponse.EnsureSuccessStatusCode();

            var getResponse = await _client.GetAsync($"objects/{objectId}");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}
