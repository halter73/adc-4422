using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace anc_4422
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var runTask = CreateWebHostBuilder(args).Build().RunAsync();


            for (var i = 0; i < 256; i++)
            {
                _ = RequestTestFileAndAbort();
            }

            await runTask;
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        private static async Task RequestTestFileAndAbort()
        {
            while (true)
            {
                try
                {
                    using (var client = new TcpClient())
                    {
                        await client.ConnectAsync(IPAddress.Loopback, 5000);

                        var stream = client.GetStream();

                        using (var writer = new StreamWriter(stream))
                        {
                            await writer.WriteAsync("GET /test.txt HTTP/1.1\r\nHost: localhost:5000\r\n\r\n");
                            await writer.FlushAsync();

                            await Task.Delay(5000);
                            client.LingerState = new LingerOption(true, 0);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Excpeption: {0}", ex);
                }
            }
        }
    }
}
