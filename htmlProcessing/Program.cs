// See https://aka.ms/new-console-template for more information

using htmlProcessing;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

var html = await Load("https://hebrewbooks.org/beis");

var cleanHtml = new Regex("\\s+").Replace(html, " ");
var htmlLines = new Regex("<(.*?)>").Split(cleanHtml).Where(s => s.Length > 0);
List<string> htmlElements = htmlLines.ToList();

HtmlElement root = BuildTree(htmlElements);

Selector selector = Selector.ParseQuery("head"); 
HashSet<HtmlElement> matchingElements = root.FindElements(selector);
foreach (HtmlElement element in matchingElements)
    Console.WriteLine(element.ToString());
Console.ReadLine();
HtmlElement BuildTree(List<string> htmlLines)
{
    HtmlElement root = new HtmlElement();
    HtmlElement current = root;

    foreach (var htmlLine in htmlLines)
    {
        if (htmlLine == "/html")
            break;

        if (htmlLine[0] == '/' && htmlLine[1] != '*')
            current = current.Parent;
        else
        {
            string tagName = htmlLine.Split(' ')[0];
            if (HtmlHelper.HtmlHelperInstance.HtmlTags.Contains(tagName) && htmlLine[htmlLine.Length - 1] != '/')
            {
                HtmlElement newElement = createNewElement(htmlLine, tagName);
                current.Children.Add(newElement);
                newElement.Parent = current;
                current = newElement;
            }

            else
            if (HtmlHelper.HtmlHelperInstance.HtmlVoidTags.Contains(tagName))
            {
                HtmlElement newElement = createNewElement(htmlLine, tagName);
                current.Children.Add(newElement);
                newElement.Parent = current;
            }

            else
                current.InnerHtml += htmlLine;
        }
    }
    return root.Children[0];
}

HtmlElement createNewElement(string line, string tagName)
{
    HtmlElement newEl = new HtmlElement();
    var attributes = new Regex("\\b(\\w+)\\s*=\\s*\"([^\"]*)\"").Matches(line).Cast<Match>().ToList();
    var id = new Regex("id=\"(.*?)\"").Matches(line).ToList();
    if (id.Count > 0)
        newEl.Id = id.ElementAt(0).Value.Substring(4, id.ElementAt(0).Value.Length - 5);
    foreach (Match attribute in attributes)
    {
        string attributeName = attribute.Groups[1].Value;
        string attributeValue = attribute.Groups[2].Value;
        List<string> classes = new List<string>();
        if (attributeName.Equals("class", StringComparison.OrdinalIgnoreCase))
        {
            string[] wordsArray = attributeValue.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            classes.AddRange(wordsArray);
            newEl.Classes = classes;
        }
        newEl.Attributes.Add(attribute.ToString());
    }
    newEl.Name = tagName;
    return newEl;
}
async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}
