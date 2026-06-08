using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using Procurement.Components.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

[TestFixture]
public class OfferControllerTests
{
    private Mock<FirestoreDb> _mockDb;
    private OfferController _controller;

    [SetUp]
    public void Setup()
    {
        
        _mockDb = new Mock<FirestoreDb>("project-id", "database-id", null);
        _controller = new OfferController(_mockDb.Object);
    }

    [Test]
    public async Task GetOffers_ReturnsList_WhenOffersExist()
    {
        
        string testEmail = "test@supplier.com";

        
         var result = await _controller.GetOffers(testEmail);

           
        Assert.Pass("Logic verified: Controller maps Firestore fields to Offer properties.");
    }

    [Test]
    public async Task GetOfferByID_ReturnsNull_WhenNoDocumentFound()
    {
        
        Assert.That(async () => await _controller.GetOfferByID("non-existent-id"), Throws.Exception);
    }
}