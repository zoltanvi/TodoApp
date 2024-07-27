﻿namespace Modules.Tasks.Views.Controls;

/// <summary>
/// Interface for providing a way to interact with the <see cref="TaskItemViewModel"/>
/// from the <see cref="TaskItemCommandsViewModel"/>.
/// </summary>
internal interface ITaskItemViewModel
{
    void EditItem();
    void UpdateTask();
}