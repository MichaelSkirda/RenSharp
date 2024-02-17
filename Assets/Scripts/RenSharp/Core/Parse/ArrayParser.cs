using RenSharp.Models.Parse;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.RenSharp.Core.Parse
{
    public static class ArrayParser
    {
        /// <summary>
		/// Strict means that value must starts with array (ignore white spaces at start)
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
        public static ArrayParsed ParseArrayStrict(string value)
        {
            bool isInQuotes = false;
            bool started = false;
            bool commaExpected = false;

            int? closeBracketIndex = null;

            var parsedStrings = new List<string>();
            StringBuilder parsed = new StringBuilder();

            for (int i = 0; i < value.Length; i++)
            {
                char chr = value[i];

                if (started == false)
                {
                    if (char.IsWhiteSpace(chr))
                        continue;
                    if (chr == '[')
                        started = true;
                    else
                        throw new ArgumentException("value argument must start with square bracket", nameof(value));
                }

                if (chr == '"')
                {
                    if (isInQuotes == false && commaExpected)
                        throw new ArgumentException("Comma expected. Use ',' to separate array elements.");

                    if (isInQuotes)
                    {
                        parsedStrings.Add(parsed.ToString());
                        parsed = new StringBuilder();
                        commaExpected = true;
                    }

                    isInQuotes = !isInQuotes;
                    continue;
                }

                if (isInQuotes)
                {
                    parsed.Append(chr);
                }
                else
                {
                    if (char.IsWhiteSpace(chr))
                    {
                        continue;
                    }
                    else if (chr == ']')
                    {
                        closeBracketIndex = i;
                        break;
                    }
                    else if (chr == ',')
                    {
                        if (commaExpected)
                            commaExpected = false;
                        else
                            throw new ArgumentException("Unexpected comma. Use only one comma to separate array elements.");
                    }
                }
            }

            if (closeBracketIndex == null)
                throw new ArgumentException("Closing bracket (']') was not found.");

            return new ArrayParsed()
            {
                Values = parsedStrings,
                After = value.Substring(closeBracketIndex.Value + 1)
            };

        }
    }
}
