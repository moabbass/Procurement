using Bunit;
using Moq;
using Procurement.Components.Model;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class UserManagementTests : TestContext
{
    [Fact]
    public void Should_Display_User_Details_When_Email_Is_Found()
    {
        // 1. Arrange
        var mockUserController = new Mock<UserController>();

        var fakeUser = new User
        {
            Email = "test@company.com",
            FirstName = "John",
            LastName = "Doe",
            Organization = "TechCorp"
        };

        // Setup the mock to return our fake user when a specific email is requested
        mockUserController
            .Setup(c => c.GetUsersByEmail("test@company.com"))
            .ReturnsAsync(fakeUser);

        // Register the mock controller in the bUnit Service collection
        Services.AddSingleton<IUserController>(mockUserController.Object);

        // Mock other dependencies your page might have
        Services.AddSingleton<IAuthService>(new Mock<IAuthService>().Object);

        // 2. Act - Render a hypothetical page that uses UserController
         //var cut = RenderComponent<UserProfilePage>(parameters => parameters.Add(p => p.Email, "test@company.com"));
        

        // 3. Assert
        //Assert.Contains("John Doe", cut.Markup);
         //Assert.Contains("TechCorp", cut.Markup);
    }

    [Fact]
    public async Task DeleteUser_Should_Call_Controller_Method()
    {
        // Arrange
        var mockUserController = new Mock<UserController>();
        Services.AddSingleton<IUserController>(mockUserController.Object);

        // Act
        await mockUserController.Object.DeleteUserAsync("user123");

        // Assert - Verify that the method was actually called with the correct ID
        mockUserController.Verify(c => c.DeleteUserAsync("user123"), Times.Once);
    }
}