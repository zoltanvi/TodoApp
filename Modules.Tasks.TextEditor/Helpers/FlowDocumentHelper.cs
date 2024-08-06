using System.IO;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;

namespace Modules.Tasks.TextEditor.Helpers;

/// <summary>
/// Helper class for <see cref="FlowDocument"/> serialization and deserialization into and from XML.
/// </summary>
public static class FlowDocumentHelper
{
    public static string EmptySerializedDocument { get; } = XamlWriter.Save(new FlowDocument());

    public static string SerializeDocument(FlowDocument document)
    {
        var index = 0;
        List<string> documentItems = GetDocumentItems(document);
        var fixedResult = EmptySerializedDocument;

        if (documentItems.Count > 0)
        {
            var result = XamlWriter.Save(document);

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(result);

            foreach (XmlElement xmlElement in xmlDoc.DocumentElement.ChildNodes)
            {
                if (xmlElement != null)
                {
                    foreach (XmlNode xmlNode in xmlElement)
                    {
                        FixNode(xmlNode, documentItems, ref index);
                    }
                }
            }

            fixedResult = xmlDoc.OuterXml;
        }

        return fixedResult;
    }

    public static FlowDocument? DeserializeDocument(string xml)
    {
        FlowDocument? document = null;
        using var xmlReader = XmlReader.Create(new StringReader(xml));

        if (XamlReader.Load(xmlReader) is FlowDocument flowDocument)
        {
            document = flowDocument;
        }

        return document;
    }

    public static List<string> GetDocumentItems(FlowDocument document)
    {
        var documentItems = new List<string>();

        foreach (Block block in document.Blocks)
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

        return documentItems;
    }

    private static void AddListItems(List<string> documentItems, List list)
    {
        foreach (ListItem listItem in list.ListItems)
        {
            AddListItem(documentItems, listItem);
        }
    }

    private static void AddListItem(List<string> documentItems, ListItem listItem)
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

    private static void AddInlines(List<string> documentItems, Paragraph paragraph)
    {
        foreach (Inline inline in paragraph.Inlines)
        {
            AddInline(documentItems, inline);
        }
    }

    private static void AddInlines(List<string> documentItems, Span span)
    {
        foreach (Inline inline in span.Inlines)
        {
            AddInline(documentItems, inline);
        }
    }

    private static void AddInline(List<string> documentItems, Inline inline)
    {
        if (inline is Run run && !string.IsNullOrEmpty(run.Text))
        {
            documentItems.Add(run.Text);
        }
        else if (inline is Span span)
        {
            AddInlines(documentItems, span);
        }
    }

    private static void FixNode(XmlNode node, List<string> documentItems, ref int index)
    {
        if (node.HasChildNodes)
        {
            foreach (var childNode in node.ChildNodes)
            {
                if (childNode is XmlNode xmlChildNode)
                {
                    FixNode(xmlChildNode, documentItems, ref index);
                }
            }
        }
        else if (!string.IsNullOrEmpty(node.InnerText))
        {
            var item = documentItems[index++];

            // Fix the serialization bug that comes from XamlWriter.Save().
            // It messes up the '{' and '}' characters during serialization and writes '{}{' instead.
            if (node.InnerText != item)
            {
                node.InnerText = item;
            }
        }
    }
}