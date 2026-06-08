using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Procurement.Components.Model;
using Procurement.Components.Pages.Supplier;
using Xunit;

public class MakingOfferTests : TestContext
{
    private readonly Mock<BidTracker> _mockBidTracker;
    private readonly Mock<IFileService> _mockFileService; // Assuming Interface
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly Mock<IUserController> _mockUserController;
    private readonly Mock<PDFValidatorService> _PDFValidatorService;

    public MakingOfferTests()
    {
        // Initialize Mocks
        _mockBidTracker = new Mock<BidTracker>(); // Note: Still mocking the class
        _mockFileService = new Mock<IFileService>(); // Mocking the Interface (Good!)
        _mockAuthService = new Mock<IAuthService>();
        _mockConfig = new Mock<IConfiguration>();
        _mockUserController = new Mock<IUserController>();
        _PDFValidatorService = new Mock<PDFValidatorService>();

        // SETUP: To avoid the "CurrentUser" non-overridable error,
        // you MUST make the property 'virtual' in the AuthService class.
        _mockAuthService.Setup(a => a.CurrentUser).Returns(new User
        {
            Email = "supplier@test.com",
            Organization = "Test Org"
        });

        // SETUP: Mock Configuration for FileService requirements
        _mockConfig.Setup(c => c["Firebase:DB"]).Returns("test-db");
        _mockConfig.Setup(c => c["Firebase:Bucket"]).Returns("test-bucket");
        _mockConfig.Setup(c => c["Firebase:ApiKey"]).Returns("test-api-key");

        // REGISTRATION: This is where we fix the "Missing Service" errors
        // Use the explicit Class Type in the generic brackets <T>
        Services.AddSingleton<BidTracker>(_mockBidTracker.Object);
        Services.AddSingleton<IAuthService>(_mockAuthService.Object);
        Services.AddSingleton<IFileService>(_mockFileService.Object);
        Services.AddSingleton<IConfiguration>(_mockConfig.Object);
        Services.AddSingleton<PDFValidatorService>(_PDFValidatorService.Object);
    }

    [Fact]
    public void Should_Redirect_If_No_Bid_Selected()
    {
        // Arrange: Ensure bid tracker returns null/empty
        _mockBidTracker.Setup(x => x.getCurrentBidId()).Returns(string.Empty);
        var nav = Services.GetRequiredService<NavigationManager>();

        
        var cut = Render(builder => {
            builder.OpenComponent<Procurement.Components.Pages.Supplier.MakingOffer>(0);
            builder.CloseComponent();
        });

        // Assert: Verify it tries to navigate back
        Assert.Equal("http://localhost/AvailableBids", nav.Uri);
    }

    [Fact]
    public void Should_Display_Loading_State_Initially()
    {
        
        _mockBidTracker.Setup(x => x.getCurrentBidId()).Returns("bid123");

        
        var cut = Render(builder => {
            builder.OpenComponent<Procurement.Components.Pages.Supplier.MakingOffer>(0);
            builder.CloseComponent();
        });

        // Assert
        Assert.Contains("Retrieving bid specifications...", cut.Markup);
    }

    [Fact]
    public async Task Should_Show_Error_When_Submitting_Empty_Form()
    {
        // Arrange
        _mockBidTracker.Setup(x => x.getCurrentBidId()).Returns("bid123");
       
        var cut = Render(builder => {
            builder.OpenComponent<Procurement.Components.Pages.Supplier.MakingOffer>(0);
            builder.CloseComponent();
        });

        //  Find and click the submit button
        var submitBtn = cut.Find("button[type='submit']");
        await submitBtn.ClickAsync();

        // Assert
        var errorDiv = cut.Find(".alert-danger");
        Assert.Contains("You need to upload supporting document", errorDiv.InnerHtml);
    }

    [Fact]
    public void Fields_Should_Bind_To_Model()
    {
        
        _mockBidTracker.Setup(x => x.getCurrentBidId()).Returns("bid123");

        var cut = Render(builder => {
            builder.OpenComponent<Procurement.Components.Pages.Supplier.MakingOffer>(0);
            builder.CloseComponent();
        });

        
        var input = cut.Find("input[type='number']"); 
        input.Change(5000);

        Assert.Contains("5000", cut.Markup);
    }
}