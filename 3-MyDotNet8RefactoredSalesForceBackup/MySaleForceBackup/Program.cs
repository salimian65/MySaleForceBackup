
using MySaleForceBackup;


var builder = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {

                new Bootstrap(services, hostContext.Configuration, args).WireUp();
            });

var app = builder.Build();
await app.Services.GetRequiredService<MyApp>().StartAsync(args);
Console.WriteLine("Mehrdad Done!");


//args = "-u 1 -p 1 -t 1 -h 1 -a 1 -y 1 -z 1 -s 1".Split(' ');// sss.Split(' ');

    //var builder = Host.CreateApplicationBuilder(args);
    //builder.Services.AddHostedService<Worker>();
    //builder.Services.AddSingleton<MyApp>();
    //new Bootstrap(builder.Services).WireUp();
    //builder.Services.AddTransient<IMyLogic, MyLogic>();
    //var host = builder.Build();
    //host.Run();
    //Console.WriteLine("Done!");
    //var sss=Console.ReadLine();