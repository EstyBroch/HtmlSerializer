using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Xml.Linq;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace htmlProcessing
{
    internal class HtmlHelper
    {
        private static readonly HtmlHelper _htmlHelper=new HtmlHelper();
        public static HtmlHelper HtmlHelperInstance => _htmlHelper;
        public string[] HtmlTags { get; set; }
        public string[] HtmlVoidTags { get; set; }

        private HtmlHelper()
        {
            var jsonHtmlTags = File.ReadAllText("NewFolder/HtmlTags.json");
            HtmlTags = JsonSerializer.Deserialize < string[]>(jsonHtmlTags);
            var jsonHtmlVoidTags = File.ReadAllText("NewFolder/HtmlVoidTags.json");
            HtmlVoidTags = JsonSerializer.Deserialize<string[]>(jsonHtmlTags);

           
        }
    }

}
