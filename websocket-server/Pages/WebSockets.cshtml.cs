using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace websocket_server.Pages;

public class WebSocketsModel : PageModel
{
    private readonly ILogger<WebSocketsModel> _logger;

    public WebSocketsModel(ILogger<WebSocketsModel> logger){
        this._logger = logger;
    }

    public async Task OnGet(){
        if(HttpContext.WebSockets.IsWebSocketRequest){
            _logger.LogInformation($"websocket connnection found");
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await Echo(_logger, webSocket);
        }
        else{
            _logger.LogInformation($"ERROR - no websocket connnection found");
        }
    }

    private static async Task Echo(ILogger logger, WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!receiveResult.CloseStatus.HasValue)
        {
            await webSocket.SendAsync(
                new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                receiveResult.MessageType,
                receiveResult.EndOfMessage,
                CancellationToken.None);

            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
            
            if(receiveResult.CloseStatus.HasValue){
                logger.LogInformation($"webSocket closed: {receiveResult.CloseStatus.ToString()}");
            }
        }

        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }
}

