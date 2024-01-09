// See https://aka.ms/new-console-template for more information

using htmlProcessing;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

var html = await Load("https://hebrewbooks.org/beis");

var cleanHtml = new Regex("\\s+").Replace(html, " ");
var htmlLines = new Regex("<(.*?)>").Split(cleanHtml).Where(s => s.Length > 0);
List<string> htmlElements = new List<string>();
htmlElements = htmlLines.ToList();
HtmlElement root = runTree(htmlElements);

Selector selector = Selector.ParseQuery("div"); // Replace with your actual selector
HashSet<HtmlElement> matchingElements = root.FindElements(selector);
foreach (HtmlElement element in matchingElements)
    Console.WriteLine(element.ToString());
Console.WriteLine("------------");
HtmlElement runTree(List<string> list)
{
    int i = 0;
    
    HtmlElement root = new HtmlElement();
    HtmlElement current = root;

    while (list[i] != "/html")
    {
        if (list[i].StartsWith('/'))
        {
            current = current.Parent;
            i++;
            continue;
        }

        int spaceIndex = list[i].IndexOf(" ");
        string firstPart = (spaceIndex >= 0) ? list[i].Substring(0, spaceIndex) : list[i];

        if (HtmlHelper.HtmlHelperInstance.HtmlTags.Contains(firstPart) ||
            HtmlHelper.HtmlHelperInstance.HtmlVoidTags.Contains(firstPart))
        {
            HtmlElement newElement = new HtmlElement();
            current.Children.Add(newElement);
            // var attributes = new Regex("(^[\\s]*?)=\"(.*?)\"").Matches(list[i]).Cast<Match>().ToList();
            var attributes = new Regex("\\b(\\w+)\\s*=\\s*\"([^\"]*)\"").Matches(list[i]).Cast<Match>().ToList();

            //current.Attributes = attributes;//check if need to desree classes...
            var id = new Regex("id=\"(.*?)\"").Matches(list[i]).ToList();
            var name = new Regex("name=\"(.*?)\"").Matches(list[i]).ToList();
            // var clases = new Regex("class=\"(.*?)\"").Matches(list[i]).ToList();
            if (id.Count > 0)
                newElement.Id = id.ElementAt(0).Value.Substring(4, id.ElementAt(0).Value.Length - 5);
            if (name.Count > 0)
                newElement.Name = name.ElementAt(0).Value.Substring(6, name.ElementAt(0).Value.Length - 7);

            foreach (Match attribute in attributes)
            {
                string attributeName = attribute.Groups[1].Value;
                string attributeValue = attribute.Groups[2].Value;
                List<string> classes = new List<string>();
                if (attributeName.Equals("class", StringComparison.OrdinalIgnoreCase))
                {
                    string[] wordsArray = attributeValue.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    classes.AddRange(wordsArray);
                    newElement.Classes = classes;
                }
                newElement.Attributes.Add(attribute.ToString());
            }

            newElement.Parent = current;
            current = newElement;


        }
       
            
        i++;
    }
    return root;
}
//void runTree(List<string> list)
//{
//    int i = 0;
//    string firstPart = "";
//    List<string>classes= new List<string>();
//    HtmlElement root = new HtmlElement();
//    HtmlElement current = new HtmlElement();

//    while (list[i] != "/html")
//    {
//        if (list[i].StartsWith('/'))
//        {
//            current = current.Parent;
//            break;
//        }

//        int spaceIndex = list[i].IndexOf(" ");
//        if (spaceIndex >= 0)
//            firstPart = list[i].Substring(0, spaceIndex);
//        if (HtmlHelper.HtmlHelperInstance.HtmlTags.Contains(firstPart) ||
//            HtmlHelper.HtmlHelperInstance.HtmlVoidTags.Contains(firstPart))
//        {
//            HtmlElement newElement = new HtmlElement();
//            current.Children.Add(newElement);
//            var attributes = new Regex("(^[\\s]*?)=\"(.*?)\"").Matches(list[i]).ToList();
//            var attributesArray = attributes.Select(match => match.Value).ToList();
//            int classIndex = attributesArray.FindIndex(s => s.StartsWith("class"));
//            if (classIndex >= 0)
//            {
//                string wordsAfterClass = attributesArray[classIndex].Substring(6); // +5 to skip "class"
//                string[] wordsArray = wordsAfterClass.Split(' ', StringSplitOptions.RemoveEmptyEntries);
//                classes.AddRange(wordsArray);
//                newElement.Classes = classes;
//            }
//            int idIndex = attributesArray.FindIndex(s => s.StartsWith("id"));
//            if (idIndex >= 0)
//            {
//                newElement.Id = attributesArray[idIndex].Substring(3);
//            }
//            int nameIndex = attributesArray.FindIndex(s => s.StartsWith("name"));
//            if (nameIndex >= 0)
//            {
//                newElement.Id = attributesArray[idIndex].Substring(5);
//            }
//        }
//        }





//    }

//}


//var attributes = new Regex("(^[\\s]*?)=\"(.*?)\"").Matches(htmlElement);
//Console.WriteLine(attributes);
Console.ReadLine();
async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    //Console.WriteLine(html);
    return html;
}
