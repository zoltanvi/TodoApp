using MediatR;
using Modules.Tasks.Contracts.Models;

namespace Modules.Tasks.Contracts.Cqrs.Commands;

public class UpdateTaskCommand : IRequest
{
    public required TaskItem Task { get; set; }
}
