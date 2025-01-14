namespace Company.WebApplication.FS

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
#if (FrameworkNet50)
open Microsoft.OpenApi.Models
#endif
#if (CircuitBreakerHystrixOption)
open Steeltoe.CircuitBreaker.Hystrix
#endif
#if (ConnectorMongoDbOption && Steeltoe2)
open Steeltoe.CloudFoundry.Connector.MongoDb
#endif
#if (ConnectorMongoDbOption && !Steeltoe2)
open Steeltoe.Connector.MongoDb
#endif
#if (ConnectorMySqlEfCoreOption && Steeltoe2)
open Steeltoe.CloudFoundry.Connector.MySql.EFCore
#endif
#if (ConnectorMySqlEfCoreOption && !Steeltoe2)
open Steeltoe.Connector.MySql.EFCore
#endif
#if (ConnectorMySqlOption && Steeltoe2)
open Steeltoe.CloudFoundry.Connector.MySql
#endif
#if (ConnectorMySqlOption && !Steeltoe2)
open Steeltoe.Connector.MySql
#endif
#if (ConnectorOAuthOption && Steeltoe2)
open Steeltoe.CloudFoundry.Connector.OAuth
#endif
#if (ConnectorOAuthOption && !Steeltoe2)
open Steeltoe.Connector.OAuth
#endif
#if (ConnectorPostgreSqlOption && Steeltoe2)
open Steeltoe.CloudFoundry.Connector.PostgreSql
#endif
#if (ConnectorPostgreSqlOption && !Steeltoe2)
open Steeltoe.Connector.PostgreSql
#endif
#if (ConnectorPostgreSqlEfCoreOption && Steeltoe2)
open Steeltoe.CloudFoundry.Connector.PostgreSql.EFCore
#endif
#if (ConnectorPostgreSqlEfCoreOption && !Steeltoe2)
open Steeltoe.Connector.PostgreSql.EFCore
#endif
#if (ConnectorRabbitMqOption && Steeltoe2)
open Steeltoe.CloudFoundry.Connector.RabbitMQ
#endif
#if (ConnectorRabbitMqOption && !Steeltoe2)
open Steeltoe.Connector.RabbitMQ
#endif
#if (ConnectorRedisOption && Steeltoe2)
open Steeltoe.CloudFoundry.Connector.Redis
#endif
#if (ConnectorRedisOption && !Steeltoe2)
open Steeltoe.Connector.Redis
#endif
#if (ConnectorSqlServerOption && Steeltoe2)
open Steeltoe.CloudFoundry.Connector.SqlServer
#endif
#if (ConnectorSqlServerOption && !Steeltoe2)
open Steeltoe.Connector.SqlServer
#endif
#if (DiscoveryEurekaOption)
open Steeltoe.Discovery.Client
#endif
#if (HostingCloudFoundryOption)
open Steeltoe.Extensions.Configuration.CloudFoundry
#endif
#if (Steeltoe2ManagementEndpoints)
open Steeltoe.Management.CloudFoundry
#endif
#if (Steeltoe3ManagementEndpoints)
open Steeltoe.Management.Endpoint
#endif
#if (AnyTracing)
open Steeltoe.Management.Tracing
#endif
#if (AnyEfCore)
open Company.WebApplication.FS.Models
#endif

#if (NeedsSelf)
type Startup(configuration: IConfiguration) as self =
#else
type Startup(configuration: IConfiguration) =
#endif
    member _.Configuration = configuration

    // This method gets called by the runtime. Use this method to add services to the container.
    member _.ConfigureServices(services: IServiceCollection) =
#if (ConnectorOAuthOption)
        services.AddOAuthServiceOptions(self.Configuration) |> ignore
#endif
#if (HostingCloudFoundryOption)
        services.ConfigureCloudFoundryOptions(self.Configuration) |> ignore
#endif
#if (DiscoveryEurekaOption)
        services.AddDiscoveryClient(self.Configuration) |> ignore
#endif
#if (ConnectorMongoDbOption)
        services.AddMongoClient(self.Configuration) |> ignore
#endif
#if (ConnectorMySqlOption)
        services.AddMySqlConnection(self.Configuration) |> ignore
#endif
#if (ConnectorMySqlEfCoreOption)
        services.AddDbContext<SampleContext>(fun options -> options.UseMySql(self.Configuration) |> ignore) |> ignore
#endif
#if (ConnectorPostgreSqlOption)
        services.AddPostgresConnection(self.Configuration) |> ignore
#endif
#if (ConnectorPostgreSqlEfCoreOption)
        services.AddDbContext<SampleContext>(fun options -> options.UseNpgsql(self.Configuration) |> ignore) |> ignore
#endif
#if (ConnectorRabbitMqOption)
        services.AddRabbitMQConnection(self.Configuration) |> ignore
#endif
#if (ConnectorRedisOption)
        services.AddDistributedRedisCache(self.Configuration) |> ignore
#endif
#if (ConnectorSqlServerOption)
        services.AddSqlServerConnection(self.Configuration) |> ignore
#endif
#if (CircuitBreakerHystrixOption)
        services.AddHystrixCommand<HelloHystrixCommand>("MyCircuitBreakers", self.Configuration)
        services.AddHystrixMetricsStream(self.Configuration)
#endif
#if (Steeltoe2ManagementEndpoints)
        services.AddCloudFoundryActuators(self.Configuration)
#endif
#if (Steeltoe3ManagementEndpoints)
        services.AddAllActuators(self.Configuration)
#endif
#if (AnyTracing)
#if (Steeltoe2)
        services.AddDistributedTracing(self.Configuration)
#elif (Steeltoe30)
        services.AddDistributedTracing() |> ignore
#else
        services.AddDistributedTracingAspNetCore() |> ignore
#endif
#endif
        // Add framework services.
        services.AddControllers() |> ignore
#if (FrameworkNet50)
        let info = OpenApiInfo()
        info.Title <- "Company.WebApplication.FS"
        info.Version <- "v1"
        services.AddSwaggerGen(fun c -> c.SwaggerDoc("v1", info)) |> ignore
#endif

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member _.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        if (env.IsDevelopment()) then
            app.UseDeveloperExceptionPage() |> ignore
#if (FrameworkNet50)
            app.UseSwagger() |> ignore
            app.UseSwaggerUI(fun c -> c.SwaggerEndpoint("/swagger/v1/swagger.json", "Company.WebApplication.FS")) |> ignore
#endif
#if (DiscoveryEurekaOption)
#if (Steeltoe2 || Steeltoe30)
        app.UseDiscoveryClient() |> ignore
#endif
#endif
#if (CircuitBreakerHystrixOption)
        app.UseHystrixRequestContext() |> ignore
#if (Steeltoe2 || Steeltoe30)
        app.UseHystrixMetricsStream() |> ignore
#endif
#endif
#if (Steeltoe2ManagementEndpoints)
        app.UseCloudFoundryActuators()
#endif
        app.UseHttpsRedirection()
           .UseRouting()
           .UseAuthorization()
           .UseEndpoints(fun endpoints ->
                endpoints.MapControllers() |> ignore
#if (Steeltoe3ManagementEndpoints)
                endpoints.MapAllActuators() |> ignore
#endif
            ) |> ignore
