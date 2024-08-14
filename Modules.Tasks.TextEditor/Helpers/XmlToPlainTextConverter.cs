using System.Windows.Documents;

namespace Modules.Tasks.TextEditor.Helpers;

public static class XmlToPlainTextConverter
{
    public static string ConvertToPlainText(string xmlContent)
    {
        if (string.IsNullOrEmpty(xmlContent)) return string.Empty;

        if (FlowDocumentHelper.DeserializeDocument(xmlContent) is FlowDocument flowDocument)
        {
            var plainText = FlowDocumentToPlainTextConverter.Convert(flowDocument);
            return plainText;
        }

        throw new ArgumentException($"Content is not a valid {nameof(FlowDocument)}.");
    }

    public static string ConvertToXml(string? plainTextContent)
    {
        var listOfLines = plainTextContent?.Replace("\r\n", "\r").Split("\r") ?? [];

        FlowDocument document = new FlowDocument();
        foreach (var line in listOfLines)
        {
            var paragraph = new Paragraph(new Run(line));
            document.Blocks.Add(paragraph);
        }

        return FlowDocumentHelper.SerializeDocument(document);
    }
}
