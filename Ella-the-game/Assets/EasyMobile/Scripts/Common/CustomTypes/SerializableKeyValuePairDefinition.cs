namespace EasyMobile.Internal
{
    /// <summary>
    /// Simple serializable version of <see cref="System.Collections.Generic.KeyValuePair{string, string}"/>
    /// </summary>
    public class StringStringKeyValuePair : SerializableKeyValuePair<string, string>
    {
        public StringStringKeyValuePair(string key, string value) : base(key, value) { }
    }
}
