using System;
using System.Text;
using System.Threading;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Swagger;
using FacebookWebHook.Repository;

namespace FacebookWebHook
{
    public class Startup
    {
        private WebSocket webSocket;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IRepository, Repository.Repository>();

            services.AddMvc();

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Messages API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Accept web socket requests
            app.UseWebSockets();

            app.Use(async (context, next) =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    IRepository repository = serviceProvider.GetService<IRepository>();

                    await ProcessWebSocketMessages(context, webSocket, repository, loggerFactory.CreateLogger("WebSocket"));
                }
                else
                {
                    await next();
                }
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Messages API V1");
            });

            app.UseMvc();
            app.UseFileServer();
        }

        private async Task ProcessWebSocketMessages(HttpContext context, WebSocket webSocket, IRepository repository, ILogger logger)
        {
            var buffer = new byte[1024 * 4];
            int bytesToSend;
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            bytesToSend = result.Count;
            LogFrame(logger, result, buffer);

            while (!result.CloseStatus.HasValue)
            {
                // If the client send "ServerClose", then they want a server-originated close to occur
                string content = "<<binary>>";
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    content = Encoding.UTF8.GetString(buffer, 0, result.Count);

//                    repository.Add(new RepositoryItem { Created = DateTime.Now, Message = content });

                    if (content.Equals("ServerClose"))
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing from Server", CancellationToken.None);
                        logger.LogDebug($"Sent Frame Close: {WebSocketCloseStatus.NormalClosure} Closing from Server");
                        return;
                    }
                    else if (content.Equals("ServerAbort"))
                    {
                        context.Abort();
                    }
                    else if (content.Equals("Feed"))
                    {
                        StringBuilder sb = new StringBuilder();

                        foreach (var item in repository.GetAll())
                        {
                            sb.Append(item.Message);
                        }

                        byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
                        bytes.CopyTo(buffer, 0);
                        bytesToSend = bytes.Length;
                    }
                }

                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, bytesToSend), result.MessageType, result.EndOfMessage, CancellationToken.None);
                logger.LogDebug($"Sent Frame {result.MessageType}: Len={result.Count}, Fin={result.EndOfMessage}: {content}");

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                bytesToSend = result.Count;
                LogFrame(logger, result, buffer);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        private void LogFrame(ILogger logger, WebSocketReceiveResult frame, byte[] buffer)
        {
            var close = frame.CloseStatus != null;
            string message;

            if (close)
            {
                message = $"Close: {frame.CloseStatus.Value} {frame.CloseStatusDescription}";
            }
            else
            {
                string content = "<<binary>>";
                if (frame.MessageType == WebSocketMessageType.Text)
                {
                    content = Encoding.UTF8.GetString(buffer, 0, frame.Count);
                }
                message = $"{frame.MessageType}: Len={frame.Count}, Fin={frame.EndOfMessage}: {content}";
            }

            logger.LogDebug("Received Frame " + message);
        }
    }
}
