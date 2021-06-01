using System.Threading.Tasks;
using Hatra.Messenger.Models.TokenAuth;
using Hatra.Messenger.Web.Controllers;
using Shouldly;
using Xunit;

namespace Hatra.Messenger.Web.Tests.Controllers
{
    public class HomeController_Tests: MessengerWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            await AuthenticateAsync(null, new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "123qwe"
            });

            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}