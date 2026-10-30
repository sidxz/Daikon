using AutoMapper;
using Daikon.Events.Screens;
using Daikon.EventStore.Aggregate;
using Daikon.EventStore.Handlers;
using Daikon.Shared.APIClients.MLogix;
using Daikon.Shared.DTO.MLogix;
using Daikon.Shared.VM.MLogix;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Screen.Application.Features.Commands.NewHit;
using Screen.Application.Features.Commands.NewHitBatch;
using Screen.Application.Mappings;
using Screen.Domain.Aggregates;
using Xunit;

namespace Screen.Test;

// The batch response is what the UI reports as "added": it must list only hits whose events were saved.
public class RegisterHitBatchHandlerTests
{
    private static readonly Guid Collection = Guid.NewGuid();
    private static readonly Guid Other = Guid.NewGuid();

    [Fact]
    public async Task Saved_hits_are_returned()
    {
        var result = await Handler(failing: null).Handle(Batch((Collection, 2)), default);

        Assert.Equal(2, result.Count);
        Assert.All(result, h => Assert.Equal(Collection, h.HitCollectionId));
    }

    [Fact]
    public async Task Hits_that_failed_to_save_are_not_reported_as_added()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Handler(failing: Collection).Handle(Batch((Collection, 2)), default));
    }

    [Fact]
    public async Task A_failed_collection_does_not_fake_its_hits_or_hide_the_saved_ones()
    {
        var result = await Handler(failing: Other).Handle(Batch((Collection, 2), (Other, 3)), default);

        Assert.Equal(2, result.Count);
        Assert.All(result, h => Assert.Equal(Collection, h.HitCollectionId));
    }

    private static RegisterHitBatchHandler Handler(Guid? failing)
    {
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
        var mlogix = new Mock<IMLogixAPI>();
        mlogix.Setup(m => m.RegisterBatch(It.IsAny<List<RegisterMoleculeDTO>>())).ReturnsAsync(new List<MoleculeVM>());

        var store = new Mock<IEventSourcingHandler<HitCollectionAggregate>>();
        store.Setup(s => s.GetByAsyncId(It.IsAny<Guid>())).ReturnsAsync((Guid id) => new HitCollectionAggregate(
            new HitCollectionCreatedEvent { Id = id, ScreenId = Guid.NewGuid(), Name = "col", HitCollectionType = "Confirmed" }));
        store.Setup(s => s.SaveAsync(It.IsAny<AggregateRoot>())).Returns(Task.CompletedTask);
        if (failing is Guid f)
        {
            store.Setup(s => s.SaveAsync(It.Is<AggregateRoot>(a => a.Id == f)))
                .ThrowsAsync(new InvalidOperationException("optimistic concurrency conflict"));
        }

        return new RegisterHitBatchHandler(mapper, NullLogger<RegisterHitBatchHandler>.Instance, mlogix.Object, store.Object);
    }

    private static RegisterHitBatchCommand Batch(params (Guid collection, int hits)[] groups) => new()
    {
        Commands = groups
            .SelectMany(g => Enumerable.Range(0, g.hits).Select(i => new NewHitCommand { Id = g.collection, MoleculeName = $"{g.collection:N}-{i}" }))
            .ToList(),
    };
}
