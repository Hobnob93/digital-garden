using System.Text;

namespace DigitalGarden.Shared.Components.Builders;

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
        classes = classes.Trim();
        if (classes.Length > 0)
        {
            Append(classes);
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

    public ClassBuilder Add(Func<string> value, bool condition)
    {
        if (condition)
            Append(value());

        return this;
    }

    public ClassBuilder Add(string[] values, bool condition)
    {
        if (condition)
        {
            foreach (var value in values)
            {
                Append(value);
            }
        }

        return this;
    }

    public ClassBuilder Add(string value, Func<bool> when)
    {
        if (when())
            Append(value);

        return this;
    }

    public ClassBuilder Add(Func<string> value, Func<bool> when)
    {
        if (when())
            Append(value());

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
