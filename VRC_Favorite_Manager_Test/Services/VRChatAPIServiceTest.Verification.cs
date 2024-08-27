using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using VRC_Favourite_Manager.Services;
using Windows.Media.Protection.PlayReady;
using Moq;
using Moq.Protected;
using VRC_Favourite_Manager.Common;

namespace VRC_Favorite_Manager_Test.Services
{
    [TestClass]
    public partial class VRChatAPIServiceTest_Verification
    {
        [TestClass]
        public class VRChatAPIServiceTests
        {
            private const string MockUsername = "mock_username";
            private const string MockPassword = "mock_password";
            private const string MockAuthToken = "mock_auth_token";
            private const string MockTwoFactorAuthToken = "mock_2fa_token";
            private const string MockUserId = "mock_user_id";

            [TestMethod]
            public async Task VerifyAuthTokenAsync_ValidTokens_ReturnsTrue()
            {
                // Arrange
                var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
                mockHttpMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent("{\"ok\":true}")
                    });

                var httpClient = new HttpClient(mockHttpMessageHandler.Object);
                var service = new VRChatAPIService
                {
                    _Client = httpClient
                };

                // Act
                var result = await service.VerifyAuthTokenAsync(MockAuthToken, MockTwoFactorAuthToken);

                // Assert
                Assert.IsTrue(result);
            }

            [TestMethod]
            public async Task VerifyAuthTokenAsync_MissingCredentials_ReturnsFalse()
            {
                // Arrange
                var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
                mockHttpMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        Content = new StringContent("{\"error\":{\"message\":\"Missing Credentials\",\"status_code\":401}}")
                    });

                var httpClient = new HttpClient(mockHttpMessageHandler.Object);
                var service = new VRChatAPIService
                {
                    _Client = httpClient
                };

                // Act
                var result = await service.VerifyAuthTokenAsync(MockAuthToken, MockTwoFactorAuthToken);

                // Assert
                Assert.IsFalse(result);
            }

            [TestMethod]
            public async Task VerifyAuthTokenAsync_NetworkError_ReturnsFalse()
            {
                // Arrange
                var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
                mockHttpMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                    .ThrowsAsync(new HttpRequestException("Network error"));

                var httpClient = new HttpClient(mockHttpMessageHandler.Object);
                var service = new VRChatAPIService
                {
                    _Client = httpClient
                };

                // Act
                var result = await service.VerifyAuthTokenAsync(MockAuthToken, MockTwoFactorAuthToken);

                // Assert
                Assert.IsFalse(result);
            }

            [TestMethod]
            public async Task VerifyLoginWithAuthTokenAsync_SuccessfulLogin_ReturnsTrue()
            {
                // Arrange
                var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
                mockHttpMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent("{}") // Valid empty JSON
                    });

                var httpClient = new HttpClient(mockHttpMessageHandler.Object);
                var service = new VRChatAPIService
                {
                    _Client = httpClient
                };

                // Act
                var result = await service.VerifyLoginWithAuthTokenAsync(MockAuthToken, MockTwoFactorAuthToken);

                // Assert
                Assert.IsTrue(result);
            }

            [TestMethod]
            [ExpectedException(typeof(VRCIncorrectCredentialsException))]
            public async Task VerifyLoginWithAuthTokenAsync_NetworkError_ThrowsVRCIncorrectCredentialsException()
            {
                // Arrange
                var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
                mockHttpMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                    .ThrowsAsync(new HttpRequestException("Network error"));

                var httpClient = new HttpClient(mockHttpMessageHandler.Object);
                var service = new VRChatAPIService
                {
                    _Client = httpClient
                };

                // Act
                await service.VerifyLoginWithAuthTokenAsync(MockAuthToken, MockTwoFactorAuthToken);

                // Assert is handled by ExpectedException attribute
            }
            [TestMethod]
            public async Task GetUserId_ValidResponse_ReturnsUserId()
            {
                // Arrange
                var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
                mockHttpMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent($"{{\"id\":\"{MockUserId}\"}}")
                    });

                var httpClient = new HttpClient(mockHttpMessageHandler.Object);
                var service = new VRChatAPIService
                {
                    _Client = httpClient,
                    _authToken = MockAuthToken,
                    _twoFactorAuthToken = MockTwoFactorAuthToken
                };

                // Act
                var userId = await service.GetUserId();

                // Assert
                Assert.AreEqual(MockUserId, userId);
            }

            [TestMethod]
            public async Task GetUserId_NetworkError_ReturnsNull()
            {
                // Arrange
                var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
                mockHttpMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                    .ThrowsAsync(new HttpRequestException("Network error"));

                var httpClient = new HttpClient(mockHttpMessageHandler.Object);
                var service = new VRChatAPIService
                {
                    _Client = httpClient,
                    _authToken = MockAuthToken,
                    _twoFactorAuthToken = MockTwoFactorAuthToken
                };

                // Act
                var userId = await service.GetUserId();

                // Assert
                Assert.IsNull(userId);
            }

            [TestMethod]
            public async Task GetUserId_InvalidResponse_ReturnsNull()
            {
                // Arrange
                var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
                mockHttpMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent("{\"someOtherField\":\"value\"}") // Missing 'id' field
                    });

                var httpClient = new HttpClient(mockHttpMessageHandler.Object);
                var service = new VRChatAPIService
                {
                    _Client = httpClient,
                    _authToken = MockAuthToken,
                    _twoFactorAuthToken = MockTwoFactorAuthToken
                };

                // Act
                var userId = await service.GetUserId();

                // Assert
                Assert.IsNull(userId);
            }
            [TestMethod]
            public async Task VerifyLoginAsync_ValidCredentials_ReturnsTrue()
            {
                var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
                mockHttpMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Headers = { { "Set-Cookie", "auth=mock_auth_token" } },
                        Content = new StringContent($"{{\"id\":\"{MockUserId}\"}}")
                    });

                var httpClient = new HttpClient(mockHttpMessageHandler.Object);
                var service = new VRChatAPIService
                {
                    _Client = httpClient
                };

                var result = await service.VerifyLoginAsync(MockUsername, MockPassword);

                Assert.IsTrue(result);
            }

            [TestMethod]
            public async Task VerifyLoginAsync_TwoFactorAuthEmail_ThrowsException()
            {
                var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
                mockHttpMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent("emailOtp")
                    });

                var httpClient = new HttpClient(mockHttpMessageHandler.Object);
                var service = new VRChatAPIService
                {
                    _Client = httpClient
                };

                await Assert.ThrowsExceptionAsync<VRCRequiresTwoFactorAuthException>(
                    async () => await service.VerifyLoginAsync(MockUsername, MockPassword),
                    "email");
            }

            [TestMethod]
            public async Task VerifyLoginAsync_InvalidCredentials_ThrowsException()
            {
                var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
                mockHttpMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        Content = new StringContent("Invalid credentials")
                    });

                var httpClient = new HttpClient(mockHttpMessageHandler.Object);
                var service = new VRChatAPIService
                {
                    _Client = httpClient
                };

                await Assert.ThrowsExceptionAsync<VRCIncorrectCredentialsException>(
                    async () => await service.VerifyLoginAsync(MockUsername, MockPassword));
            }

            [TestMethod]
            public async Task VerifyLoginAsync_NetworkError_ThrowsServiceUnavailableException()
            {
                var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
                mockHttpMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                    .ThrowsAsync(new HttpRequestException("Network error"));

                var httpClient = new HttpClient(mockHttpMessageHandler.Object);
                var service = new VRChatAPIService
                {
                    _Client = httpClient
                };

                await Assert.ThrowsExceptionAsync<VRCServiceUnavailableException>(
                    async () => await service.VerifyLoginAsync(MockUsername, MockPassword));
            }
        }

    }
}