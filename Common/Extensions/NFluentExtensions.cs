using NFluent;
using NFluent.Extensibility;

namespace XServices.Common.Extensions
{
    public static class NFluentExtensions
    {
        /// <summary>
        /// Checks that the string is empty.
        /// </summary>
        /// <param name="check">The fluent check.</param>
        /// <returns>
        /// A check link.
        /// </returns>
        /// <exception cref="FluentCheckException">The string is not empty.</exception>
        public static ICheckLink<ICheck<string>> IsNotNullOrWhitespace(this ICheck<string> check)
        {
            var checker = ExtensibilityHelper.ExtractChecker(check);

            return checker.ExecuteCheck(
                () =>
                {
                    if (!string.IsNullOrWhiteSpace(checker.Value)) return;

                    var errorMessage = FluentMessage.BuildMessage("The {0} is null or whitespace.")
                        .For("string")
                        .On(checker.Value).ToString();

                    throw new FluentCheckException(errorMessage);
                },
                FluentMessage.BuildMessage("The {0} should be null or whitespace.")
                    .For("string")
                    .On(checker.Value)
                    .ToString());
        }
    }
}