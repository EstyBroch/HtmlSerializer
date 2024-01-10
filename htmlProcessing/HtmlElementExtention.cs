using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace htmlProcessing
{
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
}
