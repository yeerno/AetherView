using AetherView.App.Domain.Enums;

namespace AetherView.App.Domain.Entities;

public sealed class ImageAsset
{
    public ImageAsset(
        Guid id,
        string storageKey,
        ImageTemperature temperature,
        DateTimeOffset createdAt)
    {
        DomainGuard.AgainstEmpty(id, nameof(id));

        if (!Enum.IsDefined(temperature))
        {
            throw new ArgumentOutOfRangeException(nameof(temperature));
        }

        Id = id;
        StorageKey = ValidateStorageKey(storageKey);
        Temperature = temperature;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }

    public string StorageKey { get; }

    public ImageTemperature Temperature { get; }

    public DateTimeOffset CreatedAt { get; }

    private static string ValidateStorageKey(string storageKey)
    {
        string value = DomainGuard.RequiredText(storageKey, nameof(storageKey));
        string fileName = Path.GetFileName(value);
        string opaquePart = Path.GetFileNameWithoutExtension(fileName);

        if (!string.Equals(value, fileName, StringComparison.Ordinal)
            || !Guid.TryParseExact(opaquePart, "D", out _))
        {
            throw new ArgumentException(
                "The storage key must be an opaque identifier with an optional extension.",
                nameof(storageKey));
        }

        return value;
    }
}
