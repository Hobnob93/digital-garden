using DigitalGarden.Shared.Models.Data;
using DigitalGarden.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Timer = System.Timers.Timer;

namespace DigitalGarden.Client.Components.Pages;

public partial class Home : IDisposable
{
    private const int QuoteWordDisplayDelayMs = 100;
    private const string ShowClassName = "show";

    [Inject]
    public ILifeDataProvider LifeDataProvider { get; set; } = default!;

    [PersistentState]
    public FamousQuote? QuoteOfTheDay { get; set; }

    [PersistentState]
    public string[]? QuoteWords { get; set; }

    private Queue<int> HiddenWordIndices { get; set; } = [];
    private HashSet<int> ShowWordIndices { get; set; } = [];
    private bool ShowWordsCompleted { get; set; }

    private Timer _timer = new(QuoteWordDisplayDelayMs);

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        if (QuoteOfTheDay is not null)
            return;

        QuoteOfTheDay = await LifeDataProvider.GetQuoteOfTheDay();
        QuoteWords = QuoteOfTheDay?.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
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
    }

    private void ShowNextWord()
    {
        var nextIndex = HiddenWordIndices.Dequeue();
        ShowWordIndices.Add(nextIndex);

        if (HiddenWordIndices.Count == 0)
        {
            _timer.Stop();
            _timer.Enabled = false;
            ShowWordsCompleted = true;
        }

        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        _timer.Elapsed -= WordShowTimerElapsed;
        _timer.Dispose();
    }
}
