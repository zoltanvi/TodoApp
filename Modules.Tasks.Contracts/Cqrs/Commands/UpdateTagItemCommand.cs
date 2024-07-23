using MediatR;

namespace Modules.Tasks.Contracts.Cqrs.Commands;

public class UpdateTagItemCommand : IRequest
{
    public required int TagId { get; set; }
    public required string NewName { get; set; }
    public required string Color { get; set; }
}
