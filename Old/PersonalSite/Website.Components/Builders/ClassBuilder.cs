using System.Text;

namespace Website.Components.Builders;

public struct ClassBuilder
{
    private const char SeparatorChar = ' ';

    private readonly StringBuilder _stringBuilder;

    public ClassBuilder()
    {
        _stringBuilder = new StringBuilder();
    }

    public ClassBuilder(string classes) : this()
    {
        if (classes.Length > 0)
        {
            _stringBuilder.Append(classes);

            if (classes.Last() != SeparatorChar)
            {
                _stringBuilder.Append(SeparatorChar);
            }
        }
    }

    public ClassBuilder(ClassBuilder other) : this(other.Build())
    {
    }

    public ClassBuilder Add(string value)
    {
        Append(value);

        return this;
    }

    public ClassBuilder Add(string value, bool condition)
    {
        if (condition)
            Append(value);

        return this;
    }

    public ClassBuilder Add(string value, Func<bool> condition)
    {
        if (condition())
            Append(value);

        return this;
    }

    private void Append(string value)
    {
        _stringBuilder.Append($"{value}{SeparatorChar}");
    }

    public string Build()
    {
        return _stringBuilder
            .ToString()
            .TrimEnd();
    }
}
