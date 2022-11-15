const config = {
  ServerHTTPHost: process.env.SERVER_HTTP_HOST || "http://localhost:8585",
  ServerWSHost: process.env.SERVER_WS_HOST || "ws://localhost:8686",
};

export default config;
