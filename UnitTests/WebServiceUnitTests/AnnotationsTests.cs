using DatabaseService.Modules;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

using WebService.DTOs;
using Xunit;

namespace UnitTests.WebServiceUnitTests
{
    public class AnnotationsTests
    {

        private string ApiPathAnnotations = "http://localhost:5001/api/annotations";
        private string AuthenticateUserUrl = "http://localhost:5001/api/auth";
        public string UserToken { get; set; }

       
        [Fact]
        public void User_Signup_Bad_Request()
        {
            var signupUser = new SignupUserDto
            {
                Username = "Fun",
                Password = "Birds"
            };

            var (_, statusCode) = PostData(AuthenticateUserUrl+"/users", signupUser, string.Empty);
            Assert.Equal(HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public void User_login()
        {
            var user = new LoginUserDto
            {
                Username = "Fun",
                Password = "Birds"
            };

            var (loggedInUser, statusCode) = PostData(AuthenticateUserUrl+"/tokens", user, string.Empty);

            var username = (string)loggedInUser["username"];
            var token = (string)loggedInUser["token"];
            UserToken = token;
            Assert.Equal(HttpStatusCode.OK, statusCode);
            Assert.Equal(user.Username, username);
            Assert.NotNull(token);

        }
        [Fact]
        public void Add_New_Annotation_To_Post_Question()
        {
            var newAnnotation = new AnnotationsDto
            {
                PostId = 7284,
                Body = "This is annotation 7284 made using unit test."
            };

            //Login the user in order to get the token
            UserToken = GetToken();

            var (annotation, statusCode) = PostData(ApiPathAnnotations, newAnnotation, UserToken);

            var createdAnnotBody = (string)annotation["body"];
            var annotationUrl = (string)annotation["url"];

            Assert.Equal(HttpStatusCode.OK, statusCode);
            Assert.NotNull(annotationUrl);
            Assert.Equal(createdAnnotBody, newAnnotation.Body);

        }

        private string GetToken()
        {
            var user = new LoginUserDto
            {
                Username = "Fun",
                Password = "Birds"
            };

            var (loggedInUser, statusCode) = PostData(AuthenticateUserUrl + "/tokens", user, string.Empty);
            var token = (string)loggedInUser["token"];
            return token;
        }


        // Helpers
        //(JArray, HttpStatusCode) GetArray(string url)
        //{
        //    var client = new HttpClient();
        //    var response = client.GetAsync(url).Result;
        //    var data = response.Content.ReadAsStringAsync().Result;
        //    return ((JArray)JsonConvert.DeserializeObject(data), response.StatusCode);
        //}

        //(JObject, HttpStatusCode) GetObject(string url)
        //{
        //    var client = new HttpClient();
        //    var response = client.GetAsync(url).Result;
        //    var data = response.Content.ReadAsStringAsync().Result;
        //    return ((JObject)JsonConvert.DeserializeObject(data), response.StatusCode);
        //}

        (JObject, HttpStatusCode) PostData(string url, object content, string userToken)
        {
            var client = new HttpClient();
            if (!string.IsNullOrEmpty(userToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
            }
            var requestContent = new StringContent(
                JsonConvert.SerializeObject(content),
                Encoding.UTF8,
                "application/json");
            var response = client.PostAsync(url, requestContent).Result;
            var data = response.Content.ReadAsStringAsync().Result;
            return ((JObject)JsonConvert.DeserializeObject(data), response.StatusCode);
        }

        //HttpStatusCode PutData(string url, object content)
        //{
        //    var client = new HttpClient();
        //    var response = client.PutAsync(
        //        url,
        //        new StringContent(
        //            JsonConvert.SerializeObject(content),
        //            Encoding.UTF8,
        //            "application/json")).Result;
        //    return response.StatusCode;
        //}

        //HttpStatusCode DeleteData(string url)
        //{
        //    var client = new HttpClient();
        //    var response = client.DeleteAsync(url).Result;
        //    return response.StatusCode;
        //}

    }
}
