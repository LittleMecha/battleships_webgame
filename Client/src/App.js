import React from "react";
import LoginForm from "./loginForm";
import RegisterForm from "./registerForm";
import GameForm from "./gameForm";
import config from "./config";
import { w3cwebsocket } from "websocket";

async function updateUsersList(token) {
  const requestOptions = {
    method: "POST",
    mode: "cors",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ token: token }),
  };

  const response = await fetch(
    `${config.ServerHTTPHost}/getInGameUsers`,
    requestOptions
  );

  return await response.json();
  // TODO add error handling
}

class Page extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      targetPage: "login",
      token: "",
      wsClient: null,
      players: [],
    };
    this.handlePageSwitch = this.handlePageSwitch.bind(this);
    this.handleLogin = this.handleLogin.bind(this);
    this.handleLogout = this.handleLogout.bind(this);
  }

  handlePageSwitch(page) {
    this.setState({ targetPage: page });
  }

  async handleLogout() {
    const wsClient = this.state.wsClient;
    const token = this.state.token;

    this.setState({
      targetPage: "login",
      token: "",
      wsClient: null,
      players: [],
    });

    const requestOptions = {
      method: "POST",
      mode: "cors",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ token: token }),
    };

    const response = await fetch(
      `${config.ServerHTTPHost}/logout`,
      requestOptions
    );

    if (response.status > 299) {
      const data = await response.json();
      alert(JSON.stringify(data));
    }

    if (wsClient) {
      wsClient.close();
    }
  }

  handleLogin(token) {
    const wsClient = new w3cwebsocket(`${config.ServerWSHost}/test`);

    wsClient.onopen = () => {
      const self = this;
      // Apparently - we cannot send message right away
      // Server will fail, so we need to wait a litte
      setTimeout(function () {
        wsClient.send(
          JSON.stringify({
            command: "attach",
            attach: {
              token: token,
            },
          })
        );

        self.setState({
          targetPage: self.state.targetPage,
          token: token,
          wsClient: wsClient,
          players: [],
        });
      }, 100);
    };

    wsClient.onmessage = (message) => {
      if (!message.data) {
        return;
      }

      const data = JSON.parse(message.data)

      switch (data.event) {
        case "user_list_updated":
          updateUsersList(this.state.token).then((data) => this.setState({
            targetPage: this.state.targetPage,
            token: token,
            wsClient: wsClient,
            players: data.users,
          }))
          break;
        default:
          return;
      }
    };

    wsClient.onclose = () => {
      if (this.state.token) {
        alert("Cannot connect to user session");
      }
      this.setState({
        targetPage: "login",
        token: "",
        wsClient: null,
        players: [],
      });
    };
  }

  render() {
    switch (this.state.targetPage) {
      case "login":
        return (
          <div>
            <LoginForm
              onPageSwitch={this.handlePageSwitch}
              onLogin={this.handleLogin}
            />
          </div>
        );
      case "register":
        return (
          <div>
            <RegisterForm onPageSwitch={this.handlePageSwitch} />
          </div>
        );
      case "game":
        return (
          <div>
            <GameForm
              onLogout={this.handleLogout}
              wsClient={this.state.wsClient}
              players={this.state.players}
            />
          </div>
        );
      default:
        return (
          <div>
            <LoginForm
              onPageSwitch={this.handlePageSwitch}
              onLogin={this.handleLogin}
            />
          </div>
        );
    }
  }
}

function App() {
  return (
    <div>
      <Page />
    </div>
  );
}

export default App;
