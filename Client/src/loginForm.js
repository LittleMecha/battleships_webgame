import React from "react";
import config from "./config";

class LoginForm extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      login: "",
      password: "",
    };

    this.handleChangeToRegister = this.handleChangeToRegister.bind(this);
    this.handleInputChange = this.handleInputChange.bind(this);
    this.handleAuthorize = this.handleAuthorize.bind(this);
  }

  async handleAuthorize(event) {
    event.preventDefault();
    const login = this.state.login;
    const passw = this.state.password;

    if (!login) {
      alert("Missing login");
      this.props.onPageSwitch("login");
      return;
    }

    if (!passw) {
      alert("Missing password");
      this.props.onPageSwitch("login");
      return;
    }

    const requestOptions = {
      method: "POST",
      mode: "cors",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ login: login, password: passw }),
    };
    const response = await fetch(
      `${config.ServerHTTPHost}/authorize`,
      requestOptions
    );
    const data = await response.json();

    if (response.status > 299) {
      alert(JSON.stringify(data));
      this.props.onPageSwitch("login");
    } else {
      this.props.onPageSwitch("game");
      this.props.onLogin(data.token);
    }
  }

  handleInputChange(event) {
    event.preventDefault();
    const target = event.target;
    const fieldname = target.getAttribute("fieldname");
    this.setState({
      [fieldname]: target.value,
    });
  }

  handleChangeToRegister(event) {
    event.preventDefault();
    this.props.onPageSwitch("register");
  }

  render() {
    return (
      <form>
        <div>
          <h2>Login</h2>
        </div>
        <div>
          <label>
            Login:
            <input
              name="loginForm_login_input"
              type="text"
              fieldname="login"
              value={this.state.login}
              onChange={this.handleInputChange}
            />
          </label>
        </div>
        <div>
          <label>
            Password:
            <input
              name="loginForm_password_input"
              type="password"
              fieldname="password"
              value={this.state.password}
              onChange={this.handleInputChange}
            />
          </label>
        </div>
        <div>
          <button name="loginForm_login_button" onClick={this.handleAuthorize}>
            Login
          </button>
        </div>
        <div>
          <button
            name="loginForm_register_button"
            onClick={this.handleChangeToRegister}
          >
            Register
          </button>
        </div>
      </form>
    );
  }
}

export default LoginForm;
