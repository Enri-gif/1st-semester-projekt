namespace api.Models;

// Resolves the "effective" tag values for an AssignmentSheet.
//
// Overwrite rule: if the sheet itself has a non-empty value for a tag field,
// the sheet wins. Otherwise we fall back to the most common value across the
// sheet's child Assignments (so an "untagged" sheet still presents reasonable
// metadata for filtering/display).
public static class AssignmentSheetExtensions
{
    public static string EffectiveSubject(this AssignmentSheet sheet) =>
        Pick(sheet.Subject, sheet.Assignments?.Select(a => a.Subject));

    public static string EffectiveLevel(this AssignmentSheet sheet) =>
        Pick(sheet.Level, sheet.Assignments?.Select(a => a.Level));

    public static string EffectiveTopic(this AssignmentSheet sheet) =>
        Pick(sheet.Topic, sheet.Assignments?.Select(a => a.Topic));

    public static string EffectiveEducation(this AssignmentSheet sheet) =>
        Pick(sheet.Education, sheet.Assignments?.Select(a => a.Education));

    public static string EffectiveOwner(this AssignmentSheet sheet) =>
        Pick(sheet.Owner, sheet.Assignments?.Select(a => a.Owner));

    public static IReadOnlyList<string> EffectiveTags(this AssignmentSheet sheet)
    {
        if (sheet.Tags is { Count: > 0 })
        {
            return sheet.Tags;
        }
        return sheet.Assignments?
            .SelectMany(a => a.Tags ?? Enumerable.Empty<string>())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct()
            .ToList()
            ?? new List<string>();
    }

    private static string Pick(string sheetValue, IEnumerable<string>? assignmentValues)
    {
        if (!string.IsNullOrWhiteSpace(sheetValue))
        {
            return sheetValue;
        }
        if (assignmentValues == null)
        {
            return "";
        }
        return assignmentValues
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .GroupBy(v => v)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault() ?? "";
    }
}
