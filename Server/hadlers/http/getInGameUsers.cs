using System.Net;
using System.Text.Json;

public partial class HandlersHTTP {
    private struct GetInGameUsersRequestBody {
        public string token { get; set; }
    }
    private struct GetInGameUsersResponseBodyItem {
        public string id { get; set; }
        public string login { get; set; }
    }
    private struct GetInGameUsersResponseBody {
        public GetInGameUsersResponseBodyItem[] users { get; set; }        
    }
    public static void GetInGameUsers(HttpListenerRequest request, HttpListenerResponse response)
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
        GetInGameUsersRequestBody getUsersReqBody = JsonSerializer.Deserialize<GetInGameUsersRequestBody>(strBody);

        UserSession userSess = UserSession.FindByToken(getUsersReqBody.token);
            if (userSess.ID.Length == 0) {
            errorResponse(response, 401, "Missing or invalid authorization token");
            log(request, response);
            return;
        }

        UserSession[] userSessions = UserSession.FindAll();
        
        GetInGameUsersResponseBodyItem[] users = new GetInGameUsersResponseBodyItem[userSessions.Length];
        for (int i = 0; i < userSessions.Length; i++) {
            User usr = User.FindByID(userSessions[i].UserID);
            GetInGameUsersResponseBodyItem usrItem = new GetInGameUsersResponseBodyItem();
            usrItem.id = usr.ID;
            usrItem.login = usr.Username;
            users[i] = usrItem;
        }

        GetInGameUsersResponseBody getUsersResp = new GetInGameUsersResponseBody();
        getUsersResp.users = users;

        string responseString = JsonSerializer.Serialize<GetInGameUsersResponseBody>(getUsersResp);
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