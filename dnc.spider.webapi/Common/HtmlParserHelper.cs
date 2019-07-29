using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dnc.spider.webapi
{
    public class HtmlParserHelper
    {
        private HtmlParser _parser;
        private IHtmlDocument _document;

        public HtmlParserHelper(string content)
        {
            _parser = new HtmlParser();
            _document = _parser.ParseDocument(content);
        }

        public string GetText(string selectors)
        {
            var list = _document.QuerySelectorAll(selectors);
            if (list != null && list.Length > 0)
            {
                var model = list.First();
                return model.TextContent.Trim();
            }
            else
            {
                return string.Empty;
            }
        }

        public List<string> GetTextList(string selectors)
        {
            List<string> result = new List<string>();
            var list = _document.QuerySelectorAll(selectors);
            if (list != null && list.Length > 0)
            {
                foreach(var item in list)
                {
                    result.Add(item.TextContent.Trim());
                }
                return result;
            }
            else
            {
                return result;
            }
        }

        public decimal? GetDecimal(string selectors)
        {
            var list = _document.QuerySelectorAll(selectors);
            if (list != null && list.Length > 0)
            {
                var model = list.First();
                if (!string.IsNullOrWhiteSpace(model.TextContent.Trim()))
                {
                    var result = Convert.ToDecimal(model.TextContent.Trim());
                    return result;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public IHtmlCollection<IElement> QuerySelectorAll(string selectors)
        {
            var list = _document.QuerySelectorAll(selectors);
            return list;
        }
    }
}
