using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;

namespace Modules.Tasks.TextEditor.Helpers;

public static class FlowDocumentSplitByLineHelper
{
    private const string Paragraph = "Paragraph";
    private const string LineBreak = "LineBreak";
    public static string EmptySerializedDocument { get; } = XamlWriter.Save(new FlowDocument());

    public static List<string> SplitByLines(string flowDocumentXml)
    {
        var paragraphList = GetParagraphsFromDocumentContent(flowDocumentXml);
        List<string> contents = new List<string>();

        foreach (XmlNode paragraph in paragraphList)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(EmptySerializedDocument);

            var importedNode = doc.ImportNode(paragraph, true);
            doc.DocumentElement.AppendChild(importedNode);

            contents.Add(doc.OuterXml);
        }

        return contents;
    }

    public static List<XmlNode> GetParagraphsFromDocumentContent(string flowDocumentXml)
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(flowDocumentXml);

        var list = new List<XmlNode>();

        foreach (XmlNode node in doc.ChildNodes)
        {
            AddNodes(list, node, doc);
        }

        return list;
    }

    private static void AddNodes(List<XmlNode> list, XmlNode node, XmlDocument doc)
    {
        var childNodes = GetChildNodes(node);
        bool hasBlock = childNodes.Any(x => x.Name == LineBreak || x.Name == Paragraph);

        if (node.Name == Paragraph && hasBlock)
        {
            var paragraph = CreateNewParagraph(doc);

            for (int i = 0; i < childNodes.Count; i++)
            {
                if (childNodes[i].Name == LineBreak)
                {
                    list.Add(paragraph);

                    paragraph = CreateNewParagraph(doc);
                }
                else
                {
                    paragraph.AppendChild(childNodes[i]);

                    if (i == childNodes.Count - 1)
                    {
                        list.Add(paragraph);
                    }
                }
            }
        }
        else if (node.Name == Paragraph)
        {
            list.Add(node);
        }
        else
        {
            foreach (XmlNode childNode in node)
            {
                AddNodes(list, childNode, doc);
            }
        }
    }

    private static XmlElement CreateNewParagraph(XmlDocument doc)
    {
        var paragraph = doc.CreateElement(Paragraph);
        paragraph.SetAttribute("xmlns", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
        return paragraph;
    }

    private static List<XmlNode> GetChildNodes(XmlNode node)
    {
        List<XmlNode> nodes = new List<XmlNode>();
        
        foreach (XmlNode child in node.ChildNodes)
        {
            nodes.Add(child);
        }
        return nodes;
    }

}
