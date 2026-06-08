using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Procurement.Components.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;



// REMOVE: using Google.Api;
// REMOVE: using Microsoft.Extensions.AI;
// REMOVE: using Rhino.Mocks;

public class CreateOrganizorTests : TestContext
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<IUserController> _mockUserController;

    public CreateOrganizorTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockUserController = new Mock<IUserController>();

        
        _mockUserController.Setup(u => u.GetUsersByRole(It.IsAny<string>()))
                           .ReturnsAsync(new List<User>());

        
        _mockUserController.Setup(u => u.GetUsersByCategory(It.IsAny<string>()))
                           .ReturnsAsync(new List<User>());
        

        Services.AddSingleton<IAuthService>(_mockAuthService.Object);
        Services.AddSingleton<IUserController>(_mockUserController.Object);

        this.JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Should_Display_Organizers_On_Load()
    {
        
        var testData = new List<User> { new User(), new User() };
        _mockUserController.Setup(u => u.GetUsersByRole(It.IsAny<string>()))
                           .ReturnsAsync(testData);

        
        var cut = Render(builder => {
            builder.OpenComponent<Procurement.Components.Pages.Admin.CreateOrganizor>(0);
            builder.CloseComponent();
        });

        //  pass because 'organizers' isn't null!
        var rows = cut.FindAll("tbody tr");
        Assert.Equal(2, rows.Count);
    }

    [Fact]
    public async Task HandleRegistration_Should_Call_AuthService()
    {
        var cut = Render(builder => {
            builder.OpenComponent<Procurement.Components.Pages.Admin.CreateOrganizor>(0);
            builder.CloseComponent();
        });

        cut.Find("input[placeholder='email@example.com']").Change("new@test.com");

        
        cut.FindAll(".custom-input")[1].Change("Bob");

        cut.FindAll(".custom-input")[2].Change("Builder");

        
        cut.FindAll(".custom-input")[3].Change("SecurePass123");

        
        await cut.Find("button[type='submit']").ClickAsync();

        _mockAuthService.Verify(s => s.RegisterAsync(
            "new@test.com",
            "SecurePass123",
            "Organizor",
            "Global Tech",
            true,
            "", "", "Bob", "Builder"),
            Times.Once);
    }
}