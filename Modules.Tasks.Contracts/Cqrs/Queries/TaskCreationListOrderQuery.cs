using MediatR;

namespace Modules.Tasks.Contracts.Cqrs.Queries;

/// <summary>
/// Used to get the correct ListOrder BEFORE the task is inserted into the list.
/// </summary>
public class TaskCreationListOrderQuery : IRequest<int>
{
    public required int CategoryId { get; set; }
}
