using Microsoft.Extensions.Logging;
using Todo.Api.Services;
using Moq;
using Todo.Api.Models;
namespace Todo.Api.Tests;

[TestFixture]
public class InMemoryTodoRepositoryTests
{
    private Mock<ILogger<InMemoryTodoRepository>> mockLogger = null!;
    private InMemoryTodoRepository underTest = null!;
    
    [SetUp]
    public void Setup()
    {
        mockLogger = new();
        underTest = new InMemoryTodoRepository(mockLogger.Object);
    }

    [Test]
    public async Task AddAsyncAddsTodoItemAndReturnsThatItem()
    {
       var item = new TodoItem { Title = "Write Unit Tests", OwnerId = "Developer-1" };

       var result = await underTest.AddAsync(item);

       Assert.That(result, Is.Not.Null);
       Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
       Assert.That(result.Title, Is.EqualTo(item.Title));
       Assert.That(result.OwnerId, Is.EqualTo(item.OwnerId));
       Assert.That(result.IsDone, Is.EqualTo(false));
       Assert.That(result.CreatedAt, Is.EqualTo(DateTimeOffset.UtcNow).Within(TimeSpan.FromSeconds(5)));
       Assert.That(result.UpdatedAt, Is.EqualTo(DateTimeOffset.MinValue));
       Assert.That(result.DeletedAt, Is.EqualTo(DateTimeOffset.MinValue));
    }

    [Test]
    public async Task GetAsyncReturnsNoItemsWhenNoTodoItemsArePresent()
    {
        var result = await underTest.GetAsync(new Guid(), "Noop");

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAsyncReturnsNoItemsWhenRequestedOwnerHasNoTodoItems()
    {
        var item1 = new TodoItem { Title = "Write Unit Tests", OwnerId = "Developer-1" };
        var item2 = new TodoItem { Title = "Update ReadMe", OwnerId = "Developer-1" };

        await underTest.AddAsync(item1);
        await underTest.AddAsync(item2);

        var result = await underTest.GetAsync(new Guid(), "NoWork");

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAsyncReturnsItemByIdWhenRequestedOwnerMatchesTrueOwner()
    {
        var item1 = new TodoItem { Title = "Write Unit Tests", OwnerId = "Developer-1" };
        var item2 = new TodoItem { Title = "Update ReadMe", OwnerId = "Developer-1" };

        await underTest.AddAsync(item1);
        await underTest.AddAsync(item2);

        var result = await underTest.GetAsync(item1.Id, item1.OwnerId);
            
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(item1.Id));
        Assert.That(result.Title, Is.EqualTo(item1.Title));
        Assert.That(result.OwnerId, Is.EqualTo(item1.OwnerId));     
    }

    [Test]
    public async Task GetAsyncDoesNotReturnItemWhenRequestedOwnerDoesNotMatchTrueOwner()
    {
        var item1 = new TodoItem { Title = "Write Unit Tests", OwnerId = "Developer-1" };
        var item2 = new TodoItem { Title = "Update ReadMe", OwnerId = "Developer-2" };

        await underTest.AddAsync(item1);
        await underTest.AddAsync(item2);

        var result = await underTest.GetAsync(item1.Id, item2.OwnerId);

        //TODO: Ideally we could throw a Forbidden Exception here instead of returning null
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAllAsyncReturnsAllItemsBelongingToRequestedOwner()
    {
        var OwnerId1 = "Developer-1";
        var OwnerId2 = "Developer-2";
        var item1 = new TodoItem { Title = "Write Unit Tests", OwnerId = OwnerId1 };
        var item2 = new TodoItem { Title = "Update ReadMe", OwnerId = OwnerId2 };
        var item3 = new TodoItem { Title = "Implement Exceptions", OwnerId = OwnerId2 };

        await underTest.AddAsync(item1);
        await underTest.AddAsync(item2);
        await underTest.AddAsync(item3);

        var result = await underTest.GetAllAsync(OwnerId2);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.All(i => i.OwnerId == OwnerId2), Is.True);
        Assert.That(result.Any(i => i.Id == item2.Id), Is.True);
        Assert.That(result.Any(i => i.Id == item3.Id), Is.True);
    }

    [Test]
    public async Task UpdateAsyncReturnsUpdatedItemWhenRequestedOwnerMatchesTrueOwner()
    {
        var OwnerId1 = "Developer-1";
        var OwnerId2 = "Developer-2";
        var item1 = new TodoItem { Title = "Write Unit Tests", OwnerId = OwnerId1 };
        var item2 = new TodoItem { Title = "Update ReadMe", OwnerId = OwnerId2 };
        var item3 = new TodoItem { Title = "Implement Exceptions", OwnerId = OwnerId2 };

        await underTest.AddAsync(item1);
        await underTest.AddAsync(item2);
        await underTest.AddAsync(item3);

        item2.Title = "Update ReadMe File";
        var result = await underTest.UpdateAsync(item2);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Title, Is.EqualTo(item2.Title));
    }

    [Test]
    public async Task UpdateAsyncDoesNotUpdateItemWhenRequestedOwnerDoesNotMatchesTrueOwner()
    {
        var OwnerId1 = "Developer-1";
        var OwnerId2 = "Developer-2";
        var item1 = new TodoItem { Title = "Write Unit Tests", OwnerId = OwnerId1 };
        var item2 = new TodoItem { Title = "Update ReadMe", OwnerId = OwnerId2 };

        await underTest.AddAsync(item1);
        await underTest.AddAsync(item2);

        var updatedItem2 = new TodoItem
        {
            Id = item2.Id,
            Title = "Update ReadMe File",
            OwnerId = item1.OwnerId 
        };

        var result = await underTest.UpdateAsync(updatedItem2);

        //TODO: Ideally we should throw exception Forbidden Operation
        Assert.That(result, Is.Null);
    }


    [Test]
    public async Task DeleteAsyncDeletesItemWhenRequestedOwnerMatchesTrueOwner()
    {
        var OwnerId1 = "Developer-1";
        var OwnerId2 = "Developer-2";
        var item1 = new TodoItem { Title = "Write Unit Tests", OwnerId = OwnerId1 };
        var item2 = new TodoItem { Title = "Update ReadMe", OwnerId = OwnerId2 };
        var item3 = new TodoItem { Title = "Implement Exceptions", OwnerId = OwnerId2 };

        await underTest.AddAsync(item1);
        await underTest.AddAsync(item2);
        await underTest.AddAsync(item3);

        var result = await underTest.DeleteAsync(item1.Id,item1.OwnerId);

        // we could change delete async to return bool, but just in case if we want to use deleted item properties
        // so we are returning the deleted item
        Assert.That(result, Is.Not.Null);
        Assert.That(result.DeletedAt, Is.EqualTo(DateTimeOffset.UtcNow).Within(TimeSpan.FromSeconds(5)));

    }

    [Test]
    public async Task DeleteAsyncDoesNotDeletesItemWhenRequestedOwnerDoesNotMatchTrueOwner()
    {
        var OwnerId1 = "Developer-1";
        var OwnerId2 = "Developer-2";
        var item1 = new TodoItem { Title = "Write Unit Tests", OwnerId = OwnerId1 };
        var item2 = new TodoItem { Title = "Update ReadMe", OwnerId = OwnerId2 };
        var item3 = new TodoItem { Title = "Implement Exceptions", OwnerId = OwnerId2 };

        await underTest.AddAsync(item1);
        await underTest.AddAsync(item2);
        await underTest.AddAsync(item3);

        var result = await underTest.DeleteAsync(item1.Id, item2.OwnerId);

        // TODO: We could throw Forbidden Operation Exception
        Assert.That(result, Is.Null);
    }

}
