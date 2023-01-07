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

        private const string ApiPathAnnotations = "http://localhost:5001/api/annotations";
        private const string AuthenticateUserUrl = "http://localhost:5001/api/auth";
        private const string DbUserName = "testuser";
        private const string DbUserPassword = "12345678";
        public string UserToken { get; set; }
        
        [Fact]
        public void User_Signup_Bad_Request_AlreadySignedUp()
        {
            var signupUser = new SignupUserDto
            {
                Username = DbUserName,
                Password = DbUserPassword
            };

            var (_, statusCode) = PostData(AuthenticateUserUrl+"/users", signupUser, string.Empty);
            Assert.Equal(HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public void User_login()
        {
            var user = new LoginUserDto
            {
                Username = DbUserName,
                Password = DbUserPassword
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
            Assert.Contains(ApiPathAnnotations, annotationUrl);
            Assert.Equal(createdAnnotBody, newAnnotation.Body);

        }
        [Fact]
        public void Get_All_Annotations_Of_User()
        {
            //Login the user in order to get the token
            UserToken = GetToken();

            var (response, statusCode) = GetObject($"{ApiPathAnnotations}/user", UserToken);

            var noOfPages = (string)response["numberOfPages"];
            var previousPageUrl = (string)response["prev"];
            var nextPageUrl = (string)response["next"];
            var itemsList = (JArray)response["items"];

            Assert.Equal(HttpStatusCode.OK, statusCode);
            Assert.Null(previousPageUrl);
            Assert.Null(nextPageUrl);
            Assert.Equal("1", noOfPages);

            JObject firstItem = (JObject)itemsList[0];
            var body = (string)firstItem["body"];
            var postId = (string)firstItem["postId"];
            var questionId = (string)firstItem["questionId"];

            Assert.Equal("This is annotation 7284 made using unit test.", body);
            Assert.Equal("7284", postId);
            Assert.Equal("7284", questionId);
        }

        [Fact]
        public void Get_All_Annotations_Of_User_By_PostId()
        {
            //Login the user in order to get the token
            UserToken = GetToken();

            var (response, statusCode) = GetArray($"{ApiPathAnnotations}/post/7284", UserToken);
            Assert.Equal(HttpStatusCode.OK, statusCode);

            JObject firstItem = (JObject)response[0];
            var body = (string)firstItem["body"];
            Assert.Equal("This is annotation 7284 made using unit test.", body);
        }

        [Fact]
        public void Update_Annotation()
        {
            AnnotationsDto annotToUpdate = new AnnotationsDto
            {
                Body = "This is new annotation body for the 2nd annotation made on post with id 7284"
            };

            UserToken = GetToken();
            var newAnnotation = new AnnotationsDto
            {
                PostId = 7284,
                Body = "This is another annotation 7284 made using unit test."
            };

            var (annotation, statusCodePost) = PostData(ApiPathAnnotations, newAnnotation, UserToken);
            Assert.Equal(HttpStatusCode.OK, statusCodePost);
            var annotId = (string)annotation["annotationId"];
            var statusCodeUpdate = PutData($"{ApiPathAnnotations}/{annotId}", annotToUpdate, UserToken);

            Assert.Equal(HttpStatusCode.NoContent, statusCodeUpdate);
        }

        [Fact]
        public void Delete_Annotation_By_Id()
        {
            UserToken = GetToken();
            var newAnnotation = new AnnotationsDto
            {
                PostId = 7284,
                Body = "This is another annotation 7284 made using unit test."
            };

            var (annotation, statusCodePost) = PostData(ApiPathAnnotations, newAnnotation, UserToken);
            //var annotId = (string)annotation["annotationId"];
            var statusCode = DeleteData($"{ApiPathAnnotations}/{annotation["annotationId"]}", UserToken);
            Assert.Equal(HttpStatusCode.OK, statusCode);
        }

        ///////////////////////////////////////////////////////////////
        // Helpers
        ///////////////////////////////////////////////////////////////
        private string GetToken()
        {
            var user = new LoginUserDto
            {
                Username = DbUserName,
                Password = DbUserPassword
            };

            var (loggedInUser, statusCode) = PostData(AuthenticateUserUrl + "/tokens", user, string.Empty);
            var token = (string)loggedInUser["token"];
            return token;
        }


        (JArray, HttpStatusCode) GetArray(string url, string userToken)
        {
            var client = new HttpClient();
            if (!string.IsNullOrEmpty(userToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
            }
            var response = client.GetAsync(url).Result;
            var data = response.Content.ReadAsStringAsync().Result;
            return ((JArray)JsonConvert.DeserializeObject(data), response.StatusCode);
        }

        (JObject, HttpStatusCode) GetObject(string url, string userToken)
        {
            var client = new HttpClient();
            if (!string.IsNullOrEmpty(userToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
            }
            var response = client.GetAsync(url).Result;
            var data = response.Content.ReadAsStringAsync().Result;
            return ((JObject)JsonConvert.DeserializeObject(data), response.StatusCode);
        }

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

        HttpStatusCode PutData(string url, object content, string userToken)
        {
            var client = new HttpClient();
            if (!string.IsNullOrEmpty(userToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
            }
            var response = client.PutAsync(
                url,
                new StringContent(
                    JsonConvert.SerializeObject(content),
                    Encoding.UTF8,
                    "application/json")).Result;
            return response.StatusCode;
        }

        HttpStatusCode DeleteData(string url, string userToken)
        {
            var client = new HttpClient();
            if (!string.IsNullOrEmpty(userToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
            }
            var response = client.DeleteAsync(url).Result;
            return response.StatusCode;
        }

    }
}
