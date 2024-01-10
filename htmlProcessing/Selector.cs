
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
