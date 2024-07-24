namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: dotnet run <user-name> <server-ip>");
                return;
            }

            string name = args[0];
            string serverIp = args[1];

            using CancellationTokenSource cts = new();

            while (true)
            {
                Console.WriteLine("Enter message: (type 'Exit' to quit or type 'Send' to send messages): ");
                string input = Console.ReadLine();

                if (input.Equals("Exit", StringComparison.OrdinalIgnoreCase))
                {
                    cts.Cancel();
                    break;
                }

                if (input.Equals("Send", StringComparison.OrdinalIgnoreCase))
                {
                    Task[] tasks = new Task[10];
                    for (int i = 0; i < 10; i++)
                    {
                        int clientNumber = i + 1;
                        tasks[i] = Task.Run(() => UdpClientSingleton.Instance.SendMessageAsync(name, serverIp, clientNumber, cts.Token));
                    }

                    await Task.WhenAll(tasks);
                }
                else
                {
                    Console.WriteLine("Invalid input. Type 'Exit' to quit or 'Send' to send messages.");
                }
            }

            Console.WriteLine("Program terminated.");
        }
    }
}
