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
            _client = new HttpClient { BaseAddress = new Uri("https://api.restful-api.dev/objects") };
        }

        private async Task LogResponse(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Status Code: {response.StatusCode}");
            Console.WriteLine($"Response: {responseContent}");
        }

        private async Task<HttpResponseMessage> LogAndEnsureSuccessAsync(HttpResponseMessage response)
        {
            await LogResponse(response);
            response.EnsureSuccessStatusCode();
            return response;
        }

        [Fact]
        public async Task Test_GetAllObjects()
        {
            var response = await _client.GetAsync("objects");
            await LogAndEnsureSuccessAsync(response);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseString));
        }

        [Fact]
        public async Task Test_AddObject()
        {
            var newObject = new { Name = "Test Object", Description = "This is a test object" };
            var content = new StringContent(JsonConvert.SerializeObject(newObject), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("objects", content);
            await LogAndEnsureSuccessAsync(response);
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
            await LogAndEnsureSuccessAsync(postResponse);
            var postResponseString = await postResponse.Content.ReadAsStringAsync();
            var createdObject = JsonConvert.DeserializeObject<dynamic>(postResponseString);
            string objectId = createdObject.id;

            var getResponse = await _client.GetAsync($"objects/{objectId}");
            await LogAndEnsureSuccessAsync(getResponse);
            var getResponseString = await getResponse.Content.ReadAsStringAsync();
            var retrievedObject = JsonConvert.DeserializeObject<dynamic>(getResponseString);
            Assert.Equal(objectId, retrievedObject.id.ToString());
        }

       [Fact]
        public async Task Test_UpdateObject()
        {
            // Create a new object
            var newObject = new { name = "Test Object", data = new { color = "Black", capacity = "64 GB" } };
            var content = new StringContent(JsonConvert.SerializeObject(newObject), Encoding.UTF8, "application/json");
            var postResponse = await _client.PostAsync("objects", content);
            await LogResponse(postResponse);
            postResponse.EnsureSuccessStatusCode();
            var postResponseString = await postResponse.Content.ReadAsStringAsync();
            var createdObject = JsonConvert.DeserializeObject<dynamic>(postResponseString);
            string objectId = createdObject.id;

            // Update the created object
            var updatedObject = new { name = "Updated Test Object", data = new { color = "Red", capacity = "128 GB" } };
            var updateContent = new StringContent(JsonConvert.SerializeObject(updatedObject), Encoding.UTF8, "application/json");
            var putResponse = await _client.PutAsync($"objects/{objectId}", updateContent);
            await LogResponse(putResponse);
            putResponse.EnsureSuccessStatusCode();
            var putResponseString = await putResponse.Content.ReadAsStringAsync();
            var updatedResponseObject = JsonConvert.DeserializeObject<dynamic>(putResponseString);

            Assert.Equal("Updated Test Object", (string)updatedResponseObject.name);
        }


        [Fact]
        public async Task Test_DeleteObject()
        {
            var newObject = new { Name = "Test Object", Description = "This is a test object" };
            var content = new StringContent(JsonConvert.SerializeObject(newObject), Encoding.UTF8, "application/json");
            var postResponse = await _client.PostAsync("objects", content);
            await LogAndEnsureSuccessAsync(postResponse);
            var postResponseString = await postResponse.Content.ReadAsStringAsync();
            var createdObject = JsonConvert.DeserializeObject<dynamic>(postResponseString);
            string objectId = createdObject.id;

            var deleteResponse = await _client.DeleteAsync($"objects/{objectId}");
            await LogAndEnsureSuccessAsync(deleteResponse);

            var getResponse = await _client.GetAsync($"objects/{objectId}");
            await LogResponse(getResponse);
            Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}
