
using MySaleForceBackup;

//var builder = Host.CreateApplicationBuilder(args);
//builder.Services.AddHostedService<Worker>();
//builder.Services.AddSingleton<MyApp>();
//new Bootstrap(builder.Services).WireUp();
//builder.Services.AddTransient<IMyLogic, MyLogic>();
//var host = builder.Build();
//host.Run();
//Console.WriteLine("Done!");
var sss=Console.ReadLine();
args = sss.Split(' ');
var builder = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<MyApp>();
                new Bootstrap(services, hostContext.Configuration).WireUp();
            });
var app = builder.Build();
await app.Services.GetRequiredService<MyApp>().StartAsync();
Console.WriteLine("Mehrdad Done!");
