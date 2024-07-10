namespace OsuSpeedTest;

using System.Diagnostics;
using WindowsInput;
using WindowsInput.Native;

/// <summary>
///     Handles the hit testing.
///     Manages user input for hit testing and measures performance.
/// </summary>
public class HitTester
{
    private static int counter; // Counts the number of key presses
    private static bool running = true; // Indicates if the hit test is running
    private static VirtualKeyCode? registeredKeyCode; // The key registered for counting hits
    private const int MIN_HITS = 20; // Minimum number of hits allowed
    private const int MAX_HITS = 1000; // Maximum number of hits allowed
    private static readonly InputSimulator Simulator = new(); // Simulates input for checking key states
    private static bool keyWasReleased = true; // Tracks if key was released

    /// <summary>
    ///     Prompts the user to enter the number of hits for the test.
    ///     Ensures the entered number is within the valid range.
    /// </summary>
    /// <returns>The number of hits for the test.</returns>
    public async Task<int> GetNumberOfHitsAsync()
    {
        while (true) // Continuously prompt unit a valid input is received
        {
            try
            {
                Console.WriteLine($"Enter the number of hits (between {MIN_HITS} and {MAX_HITS}): ");
                string? input = await Task.Run(Console.ReadLine); // Reads input asynchronously

                // Checks if the input can be parsed to an integer and if it falls within the valid range
                if (int.TryParse(input, out int numHits) &&
                    numHits is >= MIN_HITS and <= MAX_HITS)
                {
                    return numHits; // Returns the valid number of hits
                }

                Console.WriteLine($"Please enter a valid number between {MIN_HITS} and {MAX_HITS}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading input: {ex.Message}");
            }
        }
    }

    /// <summary>
    ///     Tests the user's hitting speed by counting key presses.
    ///     Measures the time taken to complete the specified number of hits.
    /// </summary>
    /// <param name="numHits">The number of hits to count.</param>
    /// <returns>The time taken to complete the hits.</returns>
    public async Task<double> TestHitsAsync(int numHits)
    {
        try
        {
            counter = 0; // Resets the counter
            running = true; // Sets the running flag to true
            keyWasReleased = true; // Reset the flag for each test run

            Console.WriteLine("Press the key you want to use for counting hits.");
            var keyInfo = Console.ReadKey(true); // Registers the key to be used for counting hits
            registeredKeyCode = (VirtualKeyCode)keyInfo.Key;
            Console.WriteLine($"Registered key: {keyInfo.KeyChar}. Start hitting!");

            await ResetKeyState(); // Ensures the registered key is in a released state

            var stopwatch = new Stopwatch(); // Initializes the stopwatch

            var countTask = CountKeyPressesAsync(numHits, stopwatch); // Starts counting key presses
            await countTask; // Waits for the counting task to be complete

            ClearInputBuffer(); // Clear any residual key presses

            return stopwatch.Elapsed.TotalSeconds; // Returns the total time taken
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during hit test: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    ///     Clears any residual key presses from the input buffer.
    /// </summary>
    private static void ClearInputBuffer()
    {
        while (Console.KeyAvailable) // While there are keys available in the input buffer
        {
            Console.ReadKey(true); // Reads and discards any available key press
        }
    }

    /// <summary>
    ///     Counts the number of key presses and measures the time taken.
    /// </summary>
    /// <param name="numHits">The number of hits to count.</param>
    /// <param name="stopwatch">The stopwatch to measure time.</param>
    private static async Task CountKeyPressesAsync(int numHits, Stopwatch stopwatch)
    {
        bool firstPressDetected = false; // Tracks if the first key press was detected

        while (running) // Continuously counts key presses while the hit test is running
        {
            try
            {
                var currentState = Simulator.InputDeviceState; // Gets the current state

                // If the registered key is released 
                if (registeredKeyCode != null && currentState.IsKeyUp(registeredKeyCode.Value) && keyWasReleased == false)
                {
                    keyWasReleased = true; // Sets the key release flag 
                }

                //  If the registered key is pressed down 
                if (registeredKeyCode != null && currentState.IsKeyDown(registeredKeyCode.Value) && keyWasReleased)
                {
                    counter++; // Increments the counter for each key press
                    keyWasReleased = false; // Resets the key release flag
                    Console.WriteLine($"Hits: {counter}");

                    // Starts the stopwatch on the first key press
                    if (firstPressDetected == false)
                    {
                        stopwatch.Start();
                        firstPressDetected = true;
                    }
                }

                // Checks if the required number of hits is reached
                if (counter >= numHits)
                {
                    running = false; // Stops the counting when the required number of hits is reached
                    stopwatch.Stop(); // Stops the stopwatch
                }

                await Task.Delay(10); // Adds a small delay to reduce CPU usage
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error counting key presses: {ex.Message}");
                running = false;
            }
        }
    }

    /// <summary>
    ///     Resets the state of the registered key by waiting until it is released.
    /// </summary>
    private static async Task ResetKeyState()
    {
        while (registeredKeyCode != null && Simulator.InputDeviceState.IsKeyDown(registeredKeyCode.Value))
        {
            await Task.Delay(10); // Waits until the key is release
        }

        keyWasReleased = true; // Sets the key release flag
    }
}