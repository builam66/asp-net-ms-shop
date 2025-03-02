using Elastic.Ingest.Elasticsearch;
using Elastic.Serilog.Sinks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Common.Logging
{
    public static class SeriLogger
    {
        public static Action<HostBuilderContext, LoggerConfiguration> Configure => (context, configuration) =>
        {
            var elasticUri = context.Configuration.GetValue<string>("ElasticConfiguration:Uri")!;
            var indexFormat = $"applogs-{context.HostingEnvironment.ApplicationName?.ToLower().Replace(".", "")}_{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "")}-{DateTime.UtcNow:yyyy_MM}";

            configuration
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                .ReadFrom.Configuration(context.Configuration)
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(
                    nodes: [new Uri(elasticUri)],
                    bootstrapMethod: BootstrapMethod.Silent,
                    dataStream: indexFormat,
                    username: "elastic",
                    password: "elastic");
            //.WriteTo.Elasticsearch([new Uri(elasticUri)],
            //options =>
            //{
            //    options.DataStream = new DataStreamName(indexFormat);
            //    options.TextFormatting = new EcsTextFormatterConfiguration();
            //    options.BootstrapMethod = BootstrapMethod.Failure;
            //    options.ConfigureChannel = channelOptions =>
            //    {
            //        channelOptions.BufferOptions = new BufferOptions
            //        {
            //            ExportMaxConcurrency = 10,
            //        };
            //    };
            //    //Old: Serilog.Sinks.Elasticsearch
            //    //AutoRegisterTemplate = true,
            //    //NumberOfShards = 2,
            //    //NumberOfReplicas = 1
            //},
            //configureTransport =>
            //{
            //    configureTransport.Authentication(new BasicAuthentication("elastictest", "elastictest"));
            //    configureTransport.ServerCertificateValidationCallback((_, _, _, _) => true);
            //})
            //.CreateLogger();
        };
    }
}
