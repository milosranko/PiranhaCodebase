﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace PiranhaCMS.Search.Helpers
{
    internal static class PageContentHelpers
    {
        private static readonly Regex CleanHtmlRegex = new("<[^>]*(>|$)", RegexOptions.Compiled);
        private static readonly Regex CleanSpacesRegex = new("[\\s\\r\\n]+", RegexOptions.Compiled);

        public static string ExtractPageContent(DynamicPage page)
        {
            if (page == null) throw new ArgumentNullException(nameof(page));

            var body = new StringBuilder();

            body.AppendLine(page.Title);

            if (page.Blocks?.Count > 0)
                ExtractBlocksContent(page.Blocks, ref body);

            if (page.Regions != null)
                ExtractRegionsContent(page.Regions, ref body);

            return CleanContent(body);
        }

        public static void ExtractBlocksContent(IList<Block> blocks, ref StringBuilder sb)
        {
            if (blocks?.Count == 0 || sb == null) return;

            foreach (var block in blocks)
            {
                if (block is ISearchable searchableBlock)
                {
                    sb.AppendLine(searchableBlock.GetIndexedContent());
                }
            }
        }

        public static void ExtractRegionsContent(dynamic dynamicRegions, ref StringBuilder sb)
        {
            if (dynamicRegions == null || sb == null) return;

            if (dynamicRegions is IDictionary<string, object> regions)
            {
                foreach (var o in regions)
                {
                    if (regions[o.Key] is RegionList<ExpandoObject> regionList)
                    {
                        foreach (var fields in regionList)
                        {
                            foreach (var field in fields.Where(x => x.Value is IField))
                            {
                                sb.AppendLine(GetFieldIndexedContent(field.Value as IField));
                            }
                        }
                    }
                    else
                    {
                        if (regions[o.Key] is IDictionary<string, object> fields)
                        {
                            foreach (var field in fields.Where(x => x.Value is IField))
                            { 
                                sb.AppendLine(GetFieldIndexedContent(field.Value as IField));
                            }
                        }
                    }
                }
            }
        }

        public static string CleanContent(StringBuilder body)
        {
            return CleanSpacesRegex.Replace(CleanHtmlRegex.Replace(body.Replace("-", " ").ToString(), " "), " ").Trim();
        }

        public static string GetFieldIndexedContent(IField field)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));

            return field switch
            {
                StringField stringField => stringField.GetIndexedContent(),
                TextField textField => textField.GetIndexedContent(),
                HtmlField htmlField => htmlField.GetIndexedContent(),
                _ => string.Empty
            };
        }
    }
}
