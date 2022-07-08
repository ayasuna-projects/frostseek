using System.CommandLine;
using Ayasuna.Frostseek.Setup;
using Microsoft.Extensions.DependencyInjection;

var serviceCollection = new ServiceCollection();

serviceCollection
    .AddLoggingForFrostseek()
    .AddRootCommandForFrostseek();

var serviceProvider = serviceCollection.BuildServiceProvider();

await serviceProvider.GetRequiredService<RootCommand>().InvokeAsync(args);