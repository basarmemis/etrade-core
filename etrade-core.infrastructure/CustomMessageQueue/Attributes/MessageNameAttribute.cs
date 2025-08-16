namespace etrade_core.infrastructure.CustomMessageQueue.Attributes
{
    /// <summary>
    /// Kuyruk/Exchange adlandırma için isteğe bağlı prefix.
    /// Boş olabilir; boş ise sadece sınıf ismi baz alınır.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class MessageNamePrefixAttribute : Attribute
    {
        public string? Prefix { get; }
        public MessageNamePrefixAttribute(string? prefix = null) => Prefix = prefix;
    }
}