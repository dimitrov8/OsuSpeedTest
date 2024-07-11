namespace OsuSpeedTest;

/// <summary>
///     The main entry point.
///     Manages the main application loop and coordinates between various components.
/// </summary>
internal class StartUp
{
    static async Task Main(string[] args)
    {
        var highScoreManager = new HighScoreManager(); // Initializes the high score manager
        List<double> highScores = await highScoreManager.LoadHighScoresAsync(); // Loads the high scores from storage

        var hitTester = new HitTester(); // Initializes the hit tester component
        var feedbackProvider = new FeedbackProvider(); // Initializes the feedback provider

        while (true) // Main application loop
        {
            try
            {
                int totalHits = await GetValidNumberOfHitsAsync(highScoreManager, highScores); // Asks user for number of hits to test
                double timeTaken = await hitTester.TestHitsAsync(totalHits); // Tests the hit speed and measures time taken
                double hitsPerSecond = totalHits / timeTaken; // Calculates hits per second

                feedbackProvider.DisplayTestResults(totalHits, timeTaken, hitsPerSecond); // Display test results

                bool isNewHighScore = highScoreManager.UpdateHighScores(highScores, hitsPerSecond); // Update high scores

                // Notify user if it's a new high score
                if (isNewHighScore)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("New high score!");
                }

                Console.ResetColor();

                Console.WriteLine("Press Enter to try again or type 'exit' to quit."); // Read user input
                string? input = Console.ReadLine(); // Reads user input

                // Checks if user wants to exit the application
                if (input?.ToLower() == "exit")
                {
                    break; // Exits the application loop
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

    /// <summary>
    ///     Asynchronously prompts the user to enter a valid number of hits within a specified range.
    /// </summary>
    /// <param name="highScoreManager">The instance of the high score manager.</param>
    /// <param name="highScores">The list of high scores to display.</param>
    /// <returns>The valid number of hits entered by the user.</returns>
    private static async Task<int> GetValidNumberOfHitsAsync(HighScoreManager highScoreManager, List<double> highScores)
    {
        while (true)
        {
            Console.Clear();
            highScoreManager.DisplayHighScores(highScores); // Display current high scores
            Console.WriteLine(); // Blank line for spacing
            Console.Write($"Enter the number of hits (between {HitTester.MIN_HITS} and {HitTester.MAX_HITS}): ");
            string? input = await Task.Run(Console.ReadLine); // // Read user input asynchronously

            // Checks if the input can be parsed to an integer and if it falls within the valid range
            if (int.TryParse(input, out int totalHits) &&
                totalHits is >= HitTester.MIN_HITS and <= HitTester.MAX_HITS)
            {
                return totalHits; // Returns the valid number of hits
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Please enter a valid number between {HitTester.MIN_HITS} and {HitTester.MAX_HITS}.");
            Console.ResetColor();

            // Prompt user to retry
            Console.WriteLine();
            Console.WriteLine("Press Enter to try again...");
            await Task.Run(Console.ReadLine);
        }
    }
}