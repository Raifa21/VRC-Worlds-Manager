using Microsoft.VisualStudio.TestTools.UnitTesting;
using VRC_Favourite_Manager.Services;

namespace VRC_Favorite_Manager_Test.Services
{
    [TestClass]
    public partial class VRChatAPIServiceTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            var service = new VRChatAPIService();

            Assert.IsNotNull(service);
        }
    }
}

