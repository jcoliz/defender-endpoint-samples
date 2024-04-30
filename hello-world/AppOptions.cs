namespace HelloWorld.Options;

public record AppOptions
{
    public static readonly string Section = "App";

    public class IdentityOptions
    {
        public Guid TenantId { get; init; }

        public Guid AppId { get; init; }

        public string? AppSecret { get; init; }
    }

    public class UriOptions
    {
        public Uri? OAuth { get; init; }

        public Uri? Resources { get; init; }
    }

    public IdentityOptions? Identity { get; init; }
    public UriOptions? Uris { get; init; }
}
