using MediatR;
using Modules.Tasks.Contracts.Models;

namespace Modules.Tasks.Contracts.Cqrs.Queries;

public class TaskItemVersionsQuery : IRequest<List<TaskItemVersion>>
{
    public int TaskId { get; set; }
}
