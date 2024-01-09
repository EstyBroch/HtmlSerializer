
using htmlProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace htmlProcessing
{
    internal class Selector
    {
        public Selector()
        {
            Classes=new List<string>();
        }
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; }
        public Selector Parent { get; set; }
        public Selector Child { get; set; }
        public static Selector ParseQuery(string query)
        {
            var selectors = query.Split(' ');
            Selector root = null;
            Selector currentSelector = null;

            foreach (var selector in selectors)
            {
                //var parts = selector.Split('#', '.');
                string[] parts = Regex.Split(selector, @"(?=[.#])");
                var newSelector = new Selector();

                foreach (var part in parts)
                {
                    if (string.IsNullOrEmpty(part))
                        continue;

                    if (part.StartsWith("#"))
                    {
                        newSelector.Id = part.Remove(0, 1);
                    }
                    else if (part.StartsWith("."))
                    {
                        newSelector.Classes.Add(part.Remove(0, 1));
                    }
                  //  else if (currentSelector == null)  ////////check
                  else if(HtmlHelper.HtmlHelperInstance.HtmlTags.Contains(part) ||
                        HtmlHelper.HtmlHelperInstance.HtmlVoidTags.Contains(part))
                    {
                        newSelector.TagName = part;
                    }
                }

                if (root == null)
                {
                    root = newSelector;
                    currentSelector = root;
                }
                else
                {
                    currentSelector.Child = newSelector;
                    newSelector.Parent = currentSelector;
                    currentSelector = newSelector;
                }
            }

            return root;
        }
    }
}
internal static class HtmlElementExtensions
{
    public static HashSet<HtmlElement> FindElements(this HtmlElement element, Selector selector)
    {
        HashSet<HtmlElement> result = new HashSet<HtmlElement>();
        FindElementsRecursive(element, selector, result);
        return result;
    }

    private static void FindElementsRecursive(HtmlElement element, Selector selector, HashSet<HtmlElement> result)
    {
        if (selector == null)
        {
            // Reached the last selector, add the element to the result
            result.Add(element);
            return;
        }

        foreach (var descendantElement in element.Descendants())
        {
            if (MatchesSelector(descendantElement, selector))
            {
                if (selector.Child != null)
                {
                    FindElementsRecursive(descendantElement, selector.Child, result);
                }
                else
                {
                    result.Add(descendantElement);
                }
            }
        }
    }

    private static bool MatchesSelector(HtmlElement element, Selector selector)
    {
        if (selector.TagName != null && selector.TagName != element.Name)
        {
            return false;
        }

        if (selector.Id != null && selector.Id != element.Id)
        {
            return false;
        }

        if (selector.Classes != null && !selector.Classes.All(c => element.Classes.Contains(c)))
        {
            return false;
        }

        return true;
    }
}