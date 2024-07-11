namespace OsuSpeedTest;

/// <summary>
///     Manages the high scores.
///     Handles loading, saving, and displaying high scores.
/// </summary>
public class HighScoreManager
{
    private const string HIGH_SCORE_FILE_NAME = "OsuSpeedTest_HighScores.txt";
    private const string SEPARATOR_LINE = "---------------------------------------------";

    /// <summary>
    ///     Loads the high scores from a file asynchronously.
    /// </summary>
    /// <returns>A list of high scores.</returns>
    public async Task<List<double>> LoadHighScoresAsync()
    {
        var highScores = new List<double>();

        try
        {
            if (File.Exists(HIGH_SCORE_FILE_NAME))
            {
                string[] lines = await File.ReadAllLinesAsync(HIGH_SCORE_FILE_NAME);

                foreach (string line in lines)
                {
                    if (double.TryParse(line, out double score))
                    {
                        highScores.Add(score); // Adds each valid score to the list
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading high scores: {ex.Message}");
        }

        highScores.Sort((a, b) => b.CompareTo(a)); // Sort the high scores in descending order
        return highScores;
    }

    /// <summary>
    ///     Saves the high scores to a file asynchronously.
    /// </summary>
    /// <param name="highScores">The list of high scores to save.</param>
    public async Task SaveHighScoresAsync(List<double> highScores)
    {
        try
        {
            List<string> lines = highScores.Select(t => $"{t:F2}").ToList(); // Formats the scores as string

            await File.WriteAllLinesAsync(HIGH_SCORE_FILE_NAME, lines); // Writes the lines to the file
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving high scores: {ex.Message}");
        }
    }

    /// <summary>
    ///     Displays the high scores to the console.
    /// </summary>
    /// <param name="highScores">The list of high scores to display.</param>
    public void DisplayHighScores(List<double> highScores)
    {
        Console.Clear();
        Console.WriteLine(SEPARATOR_LINE);
        Console.WriteLine(); // Blank line for spacing
        Console.WriteLine("Osu! Speed Test");
        Console.WriteLine("GitHub profile: https://github.com/dimitrov8");
        Console.WriteLine("High scores:");

        for (int i = 0; i < highScores.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {highScores[i]:F2} hits per second"); // Displays each high score
        }

        Console.WriteLine(); // Blank line for spacing
        Console.WriteLine(SEPARATOR_LINE);
    }

    /// <summary>
    ///     Updates the list of high scores if the new score qualifies as a high score.
    /// </summary>
    /// <param name="highScores">The list of current high scores.</param>
    /// <param name="newScore">The new score to be evaluated.</param>
    /// <returns>True if the new score is a high score and the list is updated; otherwise, false.</returns>
    public bool UpdateHighScores(List<double> highScores, double newScore)
    {
        bool isNewHighScore = false;

        if (highScores.Count < 3 || newScore > highScores[^1])
        {
            highScores.Add(newScore); // Adds new high score to the list
            highScores.Sort((a, b) => b.CompareTo(a)); // Sorts high scores in descending order

            // Removes the lowest high score if more than 3 scores are saved
            if (highScores.Count > 3)
            {
                highScores.RemoveAt(highScores.Count - 1);
            }

            _ = this.SaveHighScoresAsync(highScores); // Save asynchronously without awaiting
            isNewHighScore = true;
        }

        return isNewHighScore;
    }
}