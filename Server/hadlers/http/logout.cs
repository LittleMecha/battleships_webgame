using System.Net;
using System.Text.Json;

public partial class HandlersHTTP {
    private struct LogoutRequestBody {
        public string token { get; set; }        
    }
    public static void Logout(HttpListenerRequest request, HttpListenerResponse response)
    {
        if (!request.HasEntityBody)
        {
            errorResponse(response, 400, "Missing request body");
            log(request, response);
            return;
        }
        System.IO.Stream body = request.InputStream;
        System.Text.Encoding encoding = request.ContentEncoding;
        System.IO.StreamReader reader = new System.IO.StreamReader(body, encoding);
        
        if (request.ContentType != "application/json")
        {
            errorResponse(response, 400, "Invalid content-type");
            log(request, response);
            return;
        }

        string strBody = reader.ReadToEnd();
        LogoutRequestBody logoutReq = JsonSerializer.Deserialize<LogoutRequestBody>(strBody);

        UserSession userSess = UserSession.FindByToken(logoutReq.token);
        if (userSess.ID.Length > 0) {
            if (userSess.WsSession.Length > 0) {
                WsServer.CloseSession(userSess.WsSession);
            }
            userSess.Delete();
        }

        response.StatusCode = 204;
        setDefaultHeaders(response);

        Stream responseStream = response.OutputStream;
        responseStream.Close();
        log(request, response);
    }
}