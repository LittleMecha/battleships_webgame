using System.Net;
using System.Text.Json;

public partial class HandlersHTTP {
    private struct CreateUserRequestBody {
        public string login { get; set; }
        public string password { get; set; }
    }
    private struct CreateUserResponseBody {
        public string id { get; set; }        
        public string login { get; set; }
    }
    public static void CreateUser(HttpListenerRequest request, HttpListenerResponse response)
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
        CreateUserRequestBody createUserBody = JsonSerializer.Deserialize<CreateUserRequestBody>(strBody);

        User existingUser = User.FindByUsername(createUserBody.login);
        if (existingUser.ID.Length > 0) {
            errorResponse(response, 409, "Username already in use");
            log(request, response);
            return;
        }

        string id = Commons.GetRandomID();
        User newUser = new User(id, createUserBody.login, createUserBody.password);
        newUser.Save();

        CreateUserResponseBody createUserResp = new CreateUserResponseBody();
        createUserResp.id = newUser.ID;
        createUserResp.login = newUser.Username;

        string responseString = JsonSerializer.Serialize<CreateUserResponseBody>(createUserResp);
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;

        response.StatusCode = 201;
        setDefaultHeaders(response);
        response.Headers.Set("Content-type", "application/json");

        Stream responseStream = response.OutputStream;
        responseStream.Write(buffer, 0, buffer.Length);
        responseStream.Close();
        log(request, response);
    }
}