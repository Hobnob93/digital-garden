using DigitalGarden.Shared.Models.Data;
using Microsoft.AspNetCore.Components;
using Timer = System.Timers.Timer;

namespace DigitalGarden.Shared.Components.Utility;

public partial class AnimatedFamousQuote : IDisposable
{
    private const int QuoteWordDisplayDelayMs = 100;
    private const string ShowClassName = "show";

    private readonly Timer _timer = new(QuoteWordDisplayDelayMs);

    [Parameter, EditorRequired]
    public FamousQuote Quote { get; set; }

    private string[] QuoteWords { get; set; } = [];
    private Queue<int> HiddenWordIndices { get; set; } = [];
    private HashSet<int> ShowWordIndices { get; set; } = [];
    private bool ShowWordsCompleted { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        QuoteWords = Quote.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var wordIndices = Enumerable.Range(0, QuoteWords?.Length ?? 0).ToArray();

        var random = new Random(DateTime.Now.Millisecond);
        random.Shuffle(wordIndices);
        HiddenWordIndices = new(wordIndices);

        _timer.Elapsed += WordShowTimerElapsed;
        _timer.Enabled = true;
        _timer.Start();

        ShowNextWord();

        await InvokeAsync(StateHasChanged);
    }

    private void WordShowTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        ShowNextWord();

        if (HiddenWordIndices.Count == 0)
        {
            _timer.Stop();
            _timer.Enabled = false;
            ShowWordsCompleted = true;
        }

        InvokeAsync(StateHasChanged);
    }

    private void ShowNextWord()
    {
        var nextIndex = HiddenWordIndices.Dequeue();
        ShowWordIndices.Add(nextIndex);        
    }

    public void Dispose()
    {
        _timer.Elapsed -= WordShowTimerElapsed;
        _timer.Dispose();
    }
}
