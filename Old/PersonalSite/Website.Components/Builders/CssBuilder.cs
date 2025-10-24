using System.Text;

namespace Website.Components.Builders;

public struct CssBuilder
{
    private const char KvSeparatorChar = ':';
    private const char KvpSeparatorChar = ' ';

    private readonly StringBuilder _stringBuilder;

    public CssBuilder()
    {
        _stringBuilder = new StringBuilder();
    }

    public CssBuilder(string css) : this()
    {
        if (css.Length > 0)
        {
            _stringBuilder.Append(css);

            if (css.Last() != KvpSeparatorChar)
            {
                _stringBuilder.Append(KvpSeparatorChar);
            }
        }
    }

    public CssBuilder(CssBuilder other) : this(other.Build())
    {
    }

    public CssBuilder Add(string key, string value)
    {
        Append(key, value);

        return this;
    }

    public CssBuilder Add(string key, string value, bool condition)
    {
        if (condition)
            Append(key, value);

        return this;
    }

    public CssBuilder Add(string key, string value, Func<bool> condition)
    {
        if (condition())
            Append(key, value);

        return this;
    }

    private void Append(string key, string value)
    {
        _stringBuilder.Append($"{key}{KvSeparatorChar}{value}{KvpSeparatorChar}");
    }

    public string Build()
    {
        return _stringBuilder
            .ToString()
            .Trim();
    }
}
