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

    public class LoginOptions
    {
        public Uri? Authority { get; init; }

        public ICollection<string> Scopes { get; init; } = [];
    }

    public IdentityOptions? Identity { get; init; }
    public LoginOptions? Login { get; init; }
}
