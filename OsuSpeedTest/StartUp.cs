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
                Console.Clear(); // Clears the console screen 
                Console.WriteLine("Osu! Speed Test");
                Console.WriteLine("GitHub profile: https://github.com/dimitrov8");

                highScoreManager.DisplayHighScores(highScores); // Displays the current high scores

                int numHits = await hitTester.GetNumberOfHitsAsync(); // Asks user for number of hits to test

                double timeTaken = await hitTester.TestHitsAsync(numHits); // Tests the hit speed and measures time taken
                double hitsPerSecond = numHits / timeTaken; // Calculates hits per second

                Console.ForegroundColor = ConsoleColor.DarkGray; // Set the color of the test result
                Console.WriteLine($"Your {numHits} hits have been completed in {timeTaken:F2} seconds."); // Displays test results
                Console.WriteLine($"Average hit speed: {hitsPerSecond:F2} hits per second."); // Displays average hit speed

                string feedback = feedbackProvider.GiveFeedback(hitsPerSecond, numHits); // Generates feedback based on performance
                Console.WriteLine($"Feedback: {feedback}"); // Displays feedback to the user

                bool isNewHighScore = false;

                if (highScores.Count < 10 || hitsPerSecond > highScores[^1])
                {
                    highScores.Add(hitsPerSecond); // Adds new high score to the list
                    highScores.Sort((a, b) => b.CompareTo(a)); // Sorts high scores in descending order

                    // Checks if saved high scores are more than 10
                    if (highScores.Count > 10)
                    {
                        highScores.RemoveAt(highScores.Count - 1); // Removes the lowest high score 
                    }

                    await highScoreManager.SaveHighScoresAsync(highScores); // Saves updated high scores to storage
                    isNewHighScore = true;
                }

                // Output message with color based on whether it's a new high score or not
                if (isNewHighScore)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("New high score!");
                }

                Console.ResetColor(); // Reset color to default

                Console.WriteLine("Press Enter to try again or type 'exit' to quit."); // Prompt for user input
                string input = Console.ReadLine(); // Reads user input

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
}