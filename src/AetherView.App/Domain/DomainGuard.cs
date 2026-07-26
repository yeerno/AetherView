namespace AetherView.App.Domain;

internal static class DomainGuard
{
    public static void AgainstEmpty(Guid value, string parameterName)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("A non-empty identifier is required.", parameterName);
        }
    }

    public static string RequiredText(string value, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(value, parameterName);

        string trimmedValue = value.Trim();

        if (trimmedValue.Length is 0)
        {
            throw new ArgumentException("A non-empty value is required.", parameterName);
        }

        return trimmedValue;
    }
}
