namespace OsuSpeedTest;

/// <summary>
///     Provides feedback based on the user's performance with color coding.
/// </summary>
public class FeedbackProvider
{
    /// <summary>
    ///     Gives feedback based on hits per second and total hits.
    /// </summary>
    /// <param name="hitsPerSecond">The average hits per second.</param>
    /// <param name="totalHits">The total number of hits.</param>
    /// <returns>A colored feedback string.</returns>
    public string GiveFeedback(double hitsPerSecond, int totalHits)
    {
        double performanceRatio = this.CalculatePerformanceRatio(hitsPerSecond, totalHits);
        string feedback = this.GenerateFeedback(performanceRatio);

        return feedback;
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
    ///     Generates feedback message based on performance ratio.
    /// </summary>
    /// <param name="performanceRatio">The calculated performance ratio.</param>
    /// <returns>A colored feedback message.</returns>
    private string GenerateFeedback(double performanceRatio)
    {
        string feedback;

        if (performanceRatio < 0.5)
        {
            feedback = "Keep practicing to improve your speed.";
            Console.ForegroundColor = ConsoleColor.Red;
        }
        else if (performanceRatio < 0.75)
        {
            feedback = "Good effort, you're making progress.";
            Console.ForegroundColor = ConsoleColor.Yellow;
        }
        else if (performanceRatio < 1.0)
        {
            feedback = "Very good speed, keep it up!";
            Console.ForegroundColor = ConsoleColor.Green;
        }
        else
        {
            feedback = "Excellent performance, outstanding speed!";
            Console.ForegroundColor = ConsoleColor.DarkGreen;
        }

        return feedback;
    }
}