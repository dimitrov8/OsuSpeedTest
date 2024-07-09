namespace OsuSpeedTest;

public class FeedbackProvider
{
    public string GiveFeedback(double hitsPerSecond, int totalHits)
    {
        double performanceRatio = this.CalculatePerformanceRatio(hitsPerSecond, totalHits);

        if (performanceRatio < 0.5) return "Keep practicing to improve your speed.";
        if (performanceRatio < 0.75) return "Good effort, you're making progress.";
        if (performanceRatio < 1.0) return "Very good speed, keep it up!";
        return "Excellent performance, outstanding speed!";
    }

    private double CalculatePerformanceRatio(double hitsPerSecond, int totalHits)
    {
        // This formula creates a curve that expects higher HPS for higher hit counts
        double expectedHps = 5 + (totalHits - 20) / 98.0; // Ranges from 5 HPS at 20 hits to 15 HPS at 1000 hits
        return hitsPerSecond / expectedHps;
    }
}