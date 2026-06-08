using NUnit.Framework;
using Moq;
using Google.Cloud.Firestore;
using Procurement.Components.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Google.Apis.Util;

[TestFixture]
public class UserControllerTests
{
    private Mock<FirestoreDb> _mockDb;
    private UserController _controller;

    [SetUp]
    public void Setup()
    {
       
        _mockDb = new Mock<FirestoreDb>("test-project", "test-db", null);
        _controller = new UserController(_mockDb.Object);
    }

    [Test]
    public async Task GetUsersByEmail_ReturnsNull_WhenUserDoesNotExist()
    {
       
        var result = await _controller.GetUsersByEmail("nonexistent@test.com");
        
    }

    [Test]
    public void UpdateUserAsync_ThrowsException_IfUserIdIsNull()
    {
        
        var user = new User { ID = null, Email = "test@test.com" };

        
        Assert.ThrowsAsync<ArgumentNullException>(async () => await _controller.UpdateUserAsync(user));
    }

    [Test]
    public void DeleteUserAsync_HandlesExceptions_Gracefully()
    {
        
        string fakeId = "12345";

        
        Assert.DoesNotThrowAsync(async () => await _controller.DeleteUserAsync(fakeId));
    }
}