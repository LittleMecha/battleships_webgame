using System.Net;
using System.Text.Json;

public partial class HandlersHTTP {
    private struct ErrorResponseBody {
        public string message {get; set;}
    }
    private static void setDefaultHeaders(HttpListenerResponse response) {
        response.Headers.Set("Access-Control-Allow-Origin", "*");
        response.Headers.Set("Access-Control-Allow-Credentials", "true");
        response.Headers.Set("Access-Control-Allow-Headers", "*");
        response.Headers.Set("Access-Control-Allow-Methods", "*");
    }
    private static void log(HttpListenerRequest request, HttpListenerResponse response) {
        Console.WriteLine("HTTP [{0}] {1} {2}", response.StatusCode, request.HttpMethod, request.RawUrl);
    }
    private static void errorResponse(HttpListenerResponse response, int statusCode, string message) {
        ErrorResponseBody err = new ErrorResponseBody();
        err.message = message;
        string responseString = JsonSerializer.Serialize<ErrorResponseBody>(err);
        Console.WriteLine(responseString);
        response.StatusCode = statusCode;
        
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;

        setDefaultHeaders(response);
        response.Headers.Set("Content-type", "application/json");

        Stream responseStream = response.OutputStream;
        responseStream.Write(buffer, 0, buffer.Length);
        responseStream.Close();
    }
    public static void Options(HttpListenerRequest request, HttpListenerResponse response)
    {
        setDefaultHeaders(response);
        response.StatusCode = 204;
        Stream responseStream = response.OutputStream;
        responseStream.Close();
        log(request, response);
    }
    public static void NotFound(HttpListenerRequest request, HttpListenerResponse response)
    {
        setDefaultHeaders(response);
        errorResponse(response, 404, "There's no route for this method");
        log(request, response);
    }
}
