namespace OsuSpeedTest;

/// <summary>
///     Provides feedback based on test results for the Osu! Speed Test application.
/// </summary>
public class FeedbackProvider
{
    /// <summary>
    ///     Displays the test results including hits completed, time taken, average hit speed, and feedback.
    /// </summary>
    /// <param name="totalHits">Total number of hits completed.</param>
    /// <param name="timeTaken">Time taken to complete the hits in seconds.</param>
    /// <param name="hitsPerSecond">Average hit speed in hits per second.</param>
    public void DisplayTestResults(int totalHits, double timeTaken, double hitsPerSecond)
    {
        double performanceRatio = this.CalculatePerformanceRatio(hitsPerSecond, totalHits);
        string feedback = this.GetFeedbackMessage(performanceRatio);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"Your {totalHits} hits have been completed in {timeTaken:F2} seconds.");
        Console.WriteLine($"Average hit speed: {hitsPerSecond:F2} hits per second.");

        Console.ForegroundColor = this.GetFeedbackColor(performanceRatio);
        Console.WriteLine($"Feedback: {feedback}");

        Console.ResetColor();
    }

    /// <summary>
    ///     Calculates the performance ratio based on hits per second and total hits.
    /// </summary>
    /// <param name="hitsPerSecond">The average hits per second.</param>
    /// <param name="totalHits">The total number of hits.</param>
    /// <returns>The performance ratio.</returns>
    private double CalculatePerformanceRatio(double hitsPerSecond, int totalHits)
    {
        // This formula creates a curve that expects higher HPS for higher hit counts
        double expectedHps = 5 + (totalHits - 20) / 98.0; // Ranges from 5 HPS at 20 hits to 15 HPS at 1000 hits
        return hitsPerSecond / expectedHps;
    }

    /// <summary>
    ///     Gets the feedback message based on the performance ratio.
    /// </summary>
    /// <param name="performanceRatio">The calculated performance ratio.</param>
    /// <returns>The feedback message.</returns>
    private string GetFeedbackMessage(double performanceRatio)
    {
        if (performanceRatio < 0.5)
        {
            return "Keep practicing to improve your speed.";
        }

        if (performanceRatio < 0.75)
        {
            return "Good effort, you're making progress.";
        }

        if (performanceRatio < 1.0)
        {
            return "Very good speed, keep it up!";
        }

        return "Excellent performance, outstanding speed!";
    }

    /// <summary>
    ///     Gets the console color based on the performance ratio for feedback display.
    /// </summary>
    /// <param name="performanceRatio">The calculated performance ratio.</param>
    /// <returns>The console color.</returns>
    private ConsoleColor GetFeedbackColor(double performanceRatio)
    {
        return performanceRatio switch
        {
            < 0.5 => ConsoleColor.Red,
            < 0.75 => ConsoleColor.Yellow,
            < 1.0 => ConsoleColor.Green,
            _ => ConsoleColor.DarkGreen
        };
    }
}