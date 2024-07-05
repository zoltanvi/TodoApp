using Modules.Common;
using Modules.Common.DataBinding;
using Modules.Common.ViewModel;
using Modules.Settings.Contracts.ViewModels;
using PropertyChanged;
using System.Windows.Input;

namespace Modules.Tasks.TextEditor.Controls;

[AddINotifyPropertyChangedInterface]
public class RichTextEditorViewModel : BaseViewModel
{
    private bool _isEditMode;
    private bool _enterActionOnLostFocus;
    private bool _toolbarCloseOnLostFocus;
    private string _documentContentPreview;

    public bool Focusable { get; set; }
    public bool NeedFocus { get; set; }
    public bool AcceptsTab { get; set; }
    public bool IsFormattedPasteEnabled => AppSettings.Instance.TaskPageSettings.FormattedPasteEnabled;
    public string WatermarkText { get; set; }
    public bool IsEditMode
    {
        get => _isEditMode;
        set
        {
            _isEditMode = value;
            if (FocusOnEditMode)
            {
                Focusable = value;
                NeedFocus = value;
            }
        }
    }

    public bool FocusOnEditMode { get; set; }
    public bool IsContentEmpty { get; set; }
    public string DocumentContent { get; set; }

    public string DocumentContentPreview
    {
        get => _documentContentPreview;
        set => _documentContentPreview = value;
    }

    public bool IsToolbarOpen { get; set; }
    public bool IsDisplayMode => !IsEditMode;
    public string TextColor { get; set; } = Constants.ColorName.Transparent;
    public double TextOpacity { get; set; } = 1.0;
    public Action EnterAction { get; set; }
    public Action OnQuickEditRequestedAction { get; set; }
    public ICommand LostFocusCommand { get; }

    public RichTextEditorViewModel(
        bool focusOnEditMode, 
        bool enterActionOnLostFocus, 
        bool toolbarCloseOnLostFocus, 
        bool acceptsTab)
    {
        _enterActionOnLostFocus = enterActionOnLostFocus;
        _toolbarCloseOnLostFocus = toolbarCloseOnLostFocus;
        Focusable = true;
        FocusOnEditMode = focusOnEditMode;
        LostFocusCommand = new RelayCommand(OnLostFocus);
        AcceptsTab = acceptsTab;
    }

    private void OnLostFocus()
    {
        if (_toolbarCloseOnLostFocus)
        {
            IsToolbarOpen = false;
        }

        if (_enterActionOnLostFocus)
        {
            EnterAction?.Invoke();

            IsEditMode = false;
        }
    }
}