namespace Modules.Common.Helpers;

public static class SearchHelper
{
    public static HashSet<string> GetSearchTerms(this string searchText)
    {
        return searchText
            .ToUpperInvariant()
            .Split(' ')
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToHashSet(StringComparer.InvariantCultureIgnoreCase);
    }

    public static List<string> GetSearchTermsList(this string searchText) => 
        searchText.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
}
