﻿using MediatR;

namespace Modules.PopupMessage.Contracts.Cqrs.Commands;

public abstract class ShowMessageBaseCommand : IRequest
{
    public required string Message { get; set; }
    public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(1.5);
}