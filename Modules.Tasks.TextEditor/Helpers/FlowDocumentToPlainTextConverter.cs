using System.Text;
using System.Windows.Documents;

namespace Modules.Tasks.TextEditor.Helpers;

public static class FlowDocumentToPlainTextConverter
{
    public static string ConvertToPlainText(FlowDocument document)
    {
        var documentStringBuilder = GetDocumentWithLineBreaks(document);
        return documentStringBuilder.ToString();
    }

    private static StringBuilder GetDocumentWithLineBreaks(FlowDocument document)
    {
        StringBuilder documentItems = new StringBuilder();

        foreach (Block block in document.Blocks)
        {
            if (block is Paragraph paragraph)
            {
                AddInlines(documentItems, paragraph);
                documentItems.AppendLine();
            }
            else if (block is List list)
            {
                AddListItems(documentItems, list);
            }
        }

        return documentItems;
    }

    private static void AddListItems(StringBuilder documentItems, List list)
    {
        foreach (ListItem listItem in list.ListItems)
        {
            AddListItem(documentItems, listItem);
        }
    }

    private static void AddListItem(StringBuilder documentItems, ListItem listItem)
    {
        foreach (Block block in listItem.Blocks)
        {
            if (block is Paragraph paragraph)
            {
                AddInlines(documentItems, paragraph);
            }
            else if (block is List list)
            {
                AddListItems(documentItems, list);
            }
        }
    }

    private static void AddInlines(StringBuilder documentItems, Paragraph paragraph)
    {
        foreach (Inline inline in paragraph.Inlines)
        {
            AddInline(documentItems, inline);
        }
    }

    private static void AddInlines(StringBuilder documentItems, Span span)
    {
        foreach (Inline inline in span.Inlines)
        {
            AddInline(documentItems, inline);
        }
    }

    private static void AddInline(StringBuilder documentItems, Inline inline)
    {
        if (inline is Run run && !string.IsNullOrEmpty(run.Text))
        {
            documentItems.Append(run.Text);
        }
        else if (inline is Span span)
        {
            AddInlines(documentItems, span);
        } 
        else if (inline is LineBreak)
        {
            documentItems.AppendLine();
        }
    }
}
