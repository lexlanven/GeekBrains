namespace Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: dotnet run <server-name>");
                return;
            }

            string serverName = args[0];

            UdpServerSingleton.Instance.Start(serverName);

            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            UdpServerSingleton.Instance.Stop();

            await Task.Delay(500);
        }
    }
}
