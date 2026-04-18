using Modules.Common;
using Modules.Common.DataBinding;
using Modules.Common.ViewModel;
using PropertyChanged;
using System.Windows.Input;

namespace Modules.Tasks.TextEditor.Controls;

[AddINotifyPropertyChangedInterface]
public class DynamicTextBoxViewModel : BaseViewModel
{
    private bool _isEditMode;
    private readonly bool _enterActionOnLostFocus;
    private readonly bool _toolbarCloseOnLostFocus;
    private bool _triggerFocus;

    public DynamicTextBoxViewModel(
        bool focusOnEditMode = false,
        bool enterActionOnLostFocus = false,
        bool toolbarCloseOnLostFocus = false,
        bool acceptsTab = true,
        bool isReadOnly = false,
        Action? enterAction = null)
    {
        _enterActionOnLostFocus = enterActionOnLostFocus;
        _toolbarCloseOnLostFocus = toolbarCloseOnLostFocus;
        Focusable = true;
        FocusOnEditMode = focusOnEditMode;
        LostFocusCommand = new RelayCommand(OnLostFocus);
        AcceptsTab = acceptsTab;
        IsReadOnly = isReadOnly;

        EnterAction = enterAction;
    }

    public bool TextBoxAcceptsTab { get; set; } = true;
    public bool IsReadOnly { get; set; }

    public bool Focusable { get; set; }

    public bool TriggerFocus
    {
        get => _triggerFocus;
        set
        {
            if (value)
            {
                _triggerFocus = value;
            }

            _triggerFocus = false;
        }
    }

    public bool AcceptsTab { get; set; }
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

                if (value)
                {
                    TriggerFocus = value;
                }
            }

        }
    }

    public bool FocusOnEditMode { get; set; }
    public bool IsContentEmpty { get; set; }

    public string PlainTextContent { get; set; }

    public string GetContent() => PlainTextContent;
    public string GetContentInPlainText() => PlainTextContent;

    public void SetContent(string content)
    {
        PlainTextContent = content;
    }

    public bool IsToolbarOpen { get; set; }
    public bool IsDisplayMode => !IsEditMode;
    public string TextColor { get; set; } = Constants.ColorName.Transparent;
    public double TextOpacity { get; set; } = 1.0;
    public Action OnQuickEditRequestedAction { get; set; }
    public ICommand LostFocusCommand { get; }

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

    public Action? EnterAction { get; set; }

    public bool IsEmpty => string.IsNullOrWhiteSpace(PlainTextContent);
}
