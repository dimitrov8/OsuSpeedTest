namespace OsuSpeedTest;

internal class StartUp
{
    static async Task Main(string[] args)
    {
        var highScoreManager = new HighScoreManager();
        List<double> highScores = await highScoreManager.LoadHighScoresAsync();

        var hitTester = new HitTester();
        var feedbackProvider = new FeedbackProvider();

        while (true)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("Osu! Speed Test");
                Console.WriteLine("GitHub profile: https://github.com/dimitrov8");

                highScoreManager.DisplayHighScores(highScores);

                int numHits = await hitTester.GetNumberOfHitsAsync();

                double timeTaken = await hitTester.TestHitsAsync(numHits);
                double hitsPerSecond = numHits / timeTaken;

                Console.WriteLine($"Your {numHits} hits have been completed in {timeTaken:F2} seconds.");
                Console.WriteLine($"Average hit speed: {hitsPerSecond:F2} hits per second.");

                string feedback = feedbackProvider.GiveFeedback(hitsPerSecond, numHits);
                Console.WriteLine($"Feedback: {feedback}");

                if (highScores.Count < 10 || hitsPerSecond > highScores[^1])
                {
                    highScores.Add(hitsPerSecond);
                    highScores.Sort((a, b) => b.CompareTo(a));
                    if (highScores.Count > 10)
                    {
                        highScores.RemoveAt(highScores.Count - 1);
                    }

                    await highScoreManager.SaveHighScoresAsync(highScores);
                    Console.WriteLine("New high score!");
                }

                Console.WriteLine("Press Enter to try again or type 'exit' to quit.");
                string input = Console.ReadLine();
                if (input?.ToLower() == "exit")
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
        }
    }
}