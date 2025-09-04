using antifraud_worker.Consumers;
using antifraud_worker.Configure;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.ConfigureApplication(builder.Configuration);
builder.Services.AddHostedService<KafkaConsumerWorker>();

var host = builder.Build();
host.Run();
