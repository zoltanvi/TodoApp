﻿using MediatR;
using Modules.Common.DataBinding;
using Modules.Common.ViewModel;
using Modules.Tasks.Contracts.Cqrs.Commands;
using Modules.Tasks.TextEditor.Controls;
using System.Windows.Input;

namespace Modules.RecycleBin.Views.Controls;

public class RecycleBinTaskItemViewModel : BaseViewModel
{
    public int Id { get; init; }
    public required int CategoryId { get; set; }
    public DynamicTextBoxViewModel Content { get; set; }
    public int ListOrder { get; set; }
    public bool Pinned { get; set; }
    public bool IsDone { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ModificationDate { get; set; }
    public string? MarkerColor { get; set; }
    public string? BorderColor { get; set; }
    public string? BackgroundColor { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
    public bool DetailsVisible { get; set; }

    public List<RecycleBinTaskItemVersionViewModel> Versions { get; set; }
    public int VersionCount => Versions.Count;

    public ICommand ToggleDetailsCommand { get; }
    public ICommand RestoreTaskItemCommand { get; }

    public RecycleBinTaskItemViewModel(IMediator mediator, string content, bool isContentPlainText)
    {
        ArgumentNullException.ThrowIfNull(mediator);

        ToggleDetailsCommand = new RelayCommand(() => DetailsVisible ^= true);
        RestoreTaskItemCommand = new RelayCommand(() => mediator.Send(new RestoreTaskItemCommand { TaskId = Id }));

        Content = new DynamicTextBoxViewModel(isReadOnly: true);
        Content.SetContent(isContentPlainText, content);
    }
}