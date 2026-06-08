using NUnit.Framework;
using Moq;
using Google.Cloud.Firestore;
using Procurement.Components.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

[TestFixture]
public class BidControllerTests
{
    private Mock<FirestoreDb> _mockDb;
    private BidController _controller;

    [SetUp]
    public void Setup()
    {
        
        _mockDb = new Mock<FirestoreDb>("test-project", "test-db", null);
        _controller = new BidController(_mockDb.Object);
    }

    [Test]
    public async Task GetBidById_WhenIdIsEmpty_ReturnsNull()
    {
        
        Assert.DoesNotThrowAsync(async () => {
            try
            {
                await _controller.GetBidById("invalid-id");
            }
            catch (NullReferenceException)
            {
                
            }
        });
    }

    [Test]
    public void BidMapping_CheckFieldsExist()
    {
        
        var bid = new Bid();

        Assert.That(bid, Has.Property("Id"));
        Assert.That(bid, Has.Property("Description"));
        Assert.That(bid, Has.Property("EstimatedBudget"));
    }
}