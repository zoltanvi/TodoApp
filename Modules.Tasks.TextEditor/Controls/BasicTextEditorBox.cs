using Modules.Tasks.TextEditor.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Modules.Tasks.TextEditor.Controls;

public class BasicTextEditorBox : RichTextBox
{
    private bool _setContentInProgress;
    private string _documentContent;
    private bool _serializedUpdateOnly;

    public static readonly DependencyPropertyKey IsEmptyPropertyKey = 
        DependencyProperty.RegisterReadOnly(nameof(IsEmpty), typeof(bool), typeof(BasicTextEditorBox), new PropertyMetadata());

    public static readonly DependencyProperty DocumentContentProperty =
        DependencyProperty.Register(nameof(DocumentContent), typeof(string), typeof(BasicTextEditorBox), new PropertyMetadata(OnContentChanged));

    public static readonly DependencyProperty ContentPreviewProperty = DependencyProperty.Register(
        nameof(ContentPreview), typeof(string), typeof(BasicTextEditorBox), new PropertyMetadata(default(string)));

    public string ContentPreview
    {
        get { return (string)GetValue(ContentPreviewProperty); }
        set { SetValue(ContentPreviewProperty, value); }
    }

    /// <summary>
    /// The Document of the <see cref="BasicTextEditorBox"/> serialized into xml format.
    /// </summary>
    public string DocumentContent
    {
        get => (string)GetValue(DocumentContentProperty);
        set
        {
            if (_documentContent == value)
            {
                // No need for serialization
                return;
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                SetValue(DocumentContentProperty, FlowDocumentHelper.EmptySerializedDocument);
            }
            else if (!_setContentInProgress)
            {
                _setContentInProgress = true;

                if (_serializedUpdateOnly)
                {
                    SetValue(DocumentContentProperty, value);
                    _documentContent = value;
                }
                else if (FlowDocumentHelper.DeserializeDocument(value) is FlowDocument flowDocument)
                {
                    SetValue(DocumentContentProperty, value);
                    _documentContent = value;

                    // flowDocument is cleared with the ToList() below
                    Document.Blocks.Clear();
                    Document.Blocks.AddRange(flowDocument.Blocks.ToList());
                }
                
                _setContentInProgress = false;
            }
        }
    }

    /// <summary>
    /// Indicates whether the content of the RichTextBox is empty / whitespace or not.
    /// </summary>
    public bool IsEmpty => (bool)GetValue(IsEmptyPropertyKey.DependencyProperty);

    private void SetIsEmpty(bool value) => SetValue(IsEmptyPropertyKey, value);

    public BasicTextEditorBox()
    {
        SetIsEmpty(true);
        
        WeakEventManager<RichTextBox, RoutedEventArgs>.AddHandler(this, nameof(LostFocus), OnLostFocus);
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
        base.OnGotFocus(e);

        TextChanged += OnTextChanged;
        DataObject.AddPastingHandler(this, OnPaste);
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);

        TextChanged -= OnTextChanged;
        DataObject.RemovePastingHandler(this, OnPaste);
    }

    public void UpdateContent()
    {
        _serializedUpdateOnly = true;
        DocumentContent = FlowDocumentHelper.SerializeDocument(Document);
        _serializedUpdateOnly = false;
        //SetValue(DocumentContentProperty, FlowDocumentHelper.SerializeDocument(Document));
    }

    private void SetContentPreview(FlowDocument flowDocument)
    {
        ContentPreview = FlowDocumentToPlainTextConverter.ConvertToPlainText(flowDocument);
    }

    private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BasicTextEditorBox textEditorBox && e.NewValue is string newContent)
        {
            textEditorBox.DocumentContent = newContent;
        }
    }

    private void OnLostFocus(object? sender, RoutedEventArgs e)
    {
        UpdateContent();
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        bool isEmpty = IsRichTextBoxEmpty();
        SetIsEmpty(isEmpty);
    }

    private void OnPaste(object sender, DataObjectPastingEventArgs e)
    {
        // Prevent pasting an image because it cannot be persisted.
        if (e.FormatToApply == DataFormats.Bitmap)
        {
            e.CancelCommand();
        }
    }

    private bool IsRichTextBoxEmpty()
    {
        List<string> res = FlowDocumentHelper.GetDocumentItems(Document);

        return res.Count == 0 || res.All(string.IsNullOrWhiteSpace);
    }
}
