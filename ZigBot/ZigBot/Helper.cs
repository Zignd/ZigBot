using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZigBot
{
    public static class Helper
    {
        public static string HtmlFind(HtmlNodeCollection nodes, string findNodeName, params KeyValuePair<string, string>[] findAttributes)
        {
            string result = null;

            foreach (HtmlNode node in nodes)
            {
                if (node.Name.Equals(findNodeName))
                {
                    bool[] match = new bool[findAttributes.Length];

                    for (int i = 0; i < findAttributes.Length; i++)
                    {
                        match[i] = node.Attributes.Contains(findAttributes[i].Key);
                    }

                    if (match.All(x => x.Equals(true)))
                    {
                        result = node.InnerHtml.Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<br />", "\n");
                        break;
                    }
                }
                else if (node.HasChildNodes)
                {
                    result = Helper.HtmlFind(node.ChildNodes, findNodeName, findAttributes);

                    if (result != null)
                    {
                        break;
                    }
                }
            }

            return result;
        }
    }
}
