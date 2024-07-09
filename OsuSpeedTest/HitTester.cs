namespace OsuSpeedTest;

using System.Diagnostics;

public class HitTester
{
    private static int counter;
    private static bool running = true;
    private static ConsoleKey? registeredKey;
    private const int MIN_HITS = 20;
    private const int MAX_HITS = 1000;

    public async Task<int> GetNumberOfHitsAsync()
    {
        while (true)
        {
            try
            {
                Console.WriteLine($"Enter the number of hits (between {MIN_HITS} and {MAX_HITS}): ");
                string? input = await Task.Run(() => Console.ReadLine());
                if (int.TryParse(input, out int numHits) &&
                    numHits >= MIN_HITS && numHits <= MAX_HITS)
                {
                    return numHits;
                }

                Console.WriteLine($"Please enter a valid number between {MIN_HITS} and {MAX_HITS}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading input: {ex.Message}");
            }
        }
    }

    public async Task<double> TestHitsAsync(int numHits)
    {
        try
        {
            counter = 0;
            running = true;
            registeredKey = null;

            Console.WriteLine("Press any key to start. This key will be used for counting hits.");
            registeredKey = Console.ReadKey(true).Key;
            Console.WriteLine($"Registered key: {registeredKey}. Start hitting!");
            await Task.Run(() => Console.ReadKey(true));

            Task<double> timerTask = TimerAsync(numHits);
            var countTask = CountKeyPressesAsync(numHits);
            double timeTaken = await timerTask;
            await countTask;
            return timeTaken;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during hit test: {ex.Message}");
            return 0;
        }
    }

    private static async Task<double> TimerAsync(int numHits)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        while (counter < numHits)
        {
            await Task.Delay(10);
        }

        stopwatch.Stop();
        running = false;
        return stopwatch.Elapsed.TotalSeconds;
    }

    private static async Task CountKeyPressesAsync(int numHits)
    {
        while (running)
        {
            try
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    registeredKey ??= key;

                    if (key == registeredKey)
                    {
                        counter++;
                        Console.WriteLine($"Hits: {counter}");
                    }

                    if (counter >= numHits)
                    {
                        running = false;
                    }
                }

                await Task.Delay(10);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error counting key presses: {ex.Message}");
                running = false;
            }
        }
    }
}