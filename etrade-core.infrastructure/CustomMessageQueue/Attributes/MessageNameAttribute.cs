namespace etrade_core.infrastructure.CustomMessageQueue.Attributes
{
    /// <summary>
    /// Kuyruk/Exchange adlandırma için isteğe bağlı prefix.
    /// Boş olabilir; boş ise sadece sınıf ismi baz alınır.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class MessageNameAttribute : Attribute
    {
        public string? Prefix { get; }
        public MessageNameAttribute(string? prefix = null) => Prefix = prefix;
    }
}