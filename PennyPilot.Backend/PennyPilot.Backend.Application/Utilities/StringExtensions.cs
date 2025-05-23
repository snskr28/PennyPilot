using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PennyPilot.Backend.Application.Utilities
{
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a string to PascalCase. 
        /// Example: "health care" -> "HealthCare"
        /// </summary>
        public static string ToPascalCase(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var words = input
                .Split(new[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);

            var result = new StringBuilder();
            foreach (var word in words)
            {
                if (word.Length == 0) continue;
                result.Append(char.ToUpper(word[0]));
                if (word.Length > 1)
                    result.Append(word.Substring(1).ToLower());
            }

            return result.ToString();
        }
    }
}
