using Modules.Common;
using Modules.Common.DataBinding;
using Modules.Common.ViewModel;
using Modules.Settings.Contracts.ViewModels;
using Modules.Tasks.TextEditor.Helpers;
using PropertyChanged;
using System.Windows.Input;

namespace Modules.Tasks.TextEditor.Controls;

[AddINotifyPropertyChangedInterface]
public class DynamicTextBoxViewModel : BaseViewModel
{
    private bool _isEditMode;
    private readonly bool _enterActionOnLostFocus;
    private readonly bool _toolbarCloseOnLostFocus;
    private bool _isPlainTextMode;
    private bool _triggerFocus;

    public DynamicTextBoxViewModel(
        bool focusOnEditMode = false,
        bool enterActionOnLostFocus = false,
        bool toolbarCloseOnLostFocus = false,
        bool acceptsTab = true,
        bool isPlainTextMode = true,
        Action? enterAction = null)
    {
        _enterActionOnLostFocus = enterActionOnLostFocus;
        _toolbarCloseOnLostFocus = toolbarCloseOnLostFocus;
        Focusable = true;
        FocusOnEditMode = focusOnEditMode;
        LostFocusCommand = new RelayCommand(OnLostFocus);
        AcceptsTab = acceptsTab;
        IsPlainTextMode = isPlainTextMode;

        EnterAction = enterAction;
    }

    public bool TextBoxAcceptsTab { get; set; } = true;

    public bool IsPlainTextMode
    {
        get => _isPlainTextMode;
        set
        {
            if (_isPlainTextMode)
            {
                DocumentContent = XmlToPlainTextConverter.ConvertToXml(PlainTextContent);
            }
            else
            {
                PlainTextContent = XmlToPlainTextConverter.ConvertToPlainText(DocumentContent);
            }

            _isPlainTextMode = value;
        }
    }

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

            // Auto reset to false. It is only used to notify the view about the change
            _triggerFocus = false;
        }
    }

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
    
                if (value)
                {
                    TriggerFocus = value;
                }
            }

        }
    }

    public bool FocusOnEditMode { get; set; }
    public bool IsContentEmpty { get; set; }

    public string DocumentContent { get; set; }

    public string PlainTextContent { get; set; }

    public string GetContent() => IsPlainTextMode ? PlainTextContent : DocumentContent;
    public string GetContentInPlainText() => IsPlainTextMode ? PlainTextContent : XmlToPlainTextConverter.ConvertToPlainText(DocumentContent);

    public void SetContent(bool isPlainTextContent, string content)
    {
        IsPlainTextMode = isPlainTextContent;

        if (IsPlainTextMode)
        {
            PlainTextContent = content;
        }
        else
        {
            DocumentContent = content;
        }
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

    public bool IsEmpty => IsPlainTextMode ? string.IsNullOrWhiteSpace(PlainTextContent) : IsContentEmpty;
}
