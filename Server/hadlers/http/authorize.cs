using System.Net;
using System.Text.Json;

public partial class HandlersHTTP {
    private struct AuthorizeRequestBody {
        public string login { get; set; }
        public string password { get; set; }
    }
    private struct AuthorizeResponseBody {
        public string token { get; set; }        
    }
    public static void Authorize(HttpListenerRequest request, HttpListenerResponse response)
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
        AuthorizeRequestBody authReqBody = JsonSerializer.Deserialize<AuthorizeRequestBody>(strBody);

        User existingUser = User.FindByUsername(authReqBody.login);
        if (existingUser.ID.Length == 0 || existingUser.Password != authReqBody.password) {
            errorResponse(response, 401, "Username or password are invalid");
            log(request, response);
            return;
        }

        UserSession[] sessions = UserSession.FindAll();
        
        UserSession newSession = new UserSession("", "", "", "");
        foreach (UserSession sess in sessions) {
            if (sess.UserID == existingUser.ID) {
                newSession = sess;
            }
        }

        if (sessions.Length >= 2 && newSession.ID.Length == 0) {
            errorResponse(response, 409, "Server is full, please come again later");
            log(request, response);
            return;
        }

        if (newSession.ID.Length == 0) {
            newSession.ID = Commons.GetRandomID();
            newSession.Token = Commons.GetRandomID();
            newSession.UserID = existingUser.ID;
            newSession.Save();
        }

        AuthorizeResponseBody authRes = new AuthorizeResponseBody();
        authRes.token = newSession.Token;

        string responseString = JsonSerializer.Serialize<AuthorizeResponseBody>(authRes);
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;

        response.StatusCode = 200;
        setDefaultHeaders(response);
        response.Headers.Set("Content-type", "application/json");

        Stream responseStream = response.OutputStream;
        responseStream.Write(buffer, 0, buffer.Length);
        responseStream.Close();
        log(request, response);
    }
}