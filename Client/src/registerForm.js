import React from "react";
import config from "./config";

class RegisterForm extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      login: "",
      password: "",
      confirmedPassword: "",
    };

    this.handleRegister = this.handleRegister.bind(this);
    this.handleInputChange = this.handleInputChange.bind(this);
  }

  handleInputChange(event) {
    event.preventDefault();
    const target = event.target;
    const fieldname = target.getAttribute("fieldname");
    this.setState({
      [fieldname]: target.value,
    });
  }

  async handleRegister(event) {
    event.preventDefault();
    const login = this.state.login;
    const passw = this.state.password;
    const confPassw = this.state.confirmedPassword;

    if (!login) {
      alert("Missing login");
      this.props.onPageSwitch("register");
      return;
    }

    if (!passw) {
      alert("Missing password");
      this.props.onPageSwitch("register");
      return;
    }

    if (!confPassw) {
      alert("Password not confirmed");
      this.props.onPageSwitch("register");
      return;
    }

    if (confPassw !== passw) {
      alert("Passwords does not match");
      this.props.onPageSwitch("register");
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
      `${config.ServerHTTPHost}/register`,
      requestOptions
    );
    const data = await response.json();

    if (response.status > 299) {
      alert(JSON.stringify(data));
      this.props.onPageSwitch("register");
    } else {
      alert("Registration successfull, returning to login page");
      this.props.onPageSwitch("login");
    }
  }

  render() {
    return (
      <form>
        <div>
          <h2>Register</h2>
        </div>
        <div>
          <label>
            Login:
            <input
              name="registerForm_login_input"
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
              name="registerForm_password_input"
              fieldname="password"
              type="password"
              value={this.state.password}
              onChange={this.handleInputChange}
            />
          </label>
        </div>
        <div>
          <label>
            Confirm Password:
            <input
              name="registerForm_confirmPassword_input"
              fieldname="confirmedPassword"
              type="password"
              value={this.state.confirmedPassword}
              onChange={this.handleInputChange}
            />
          </label>
        </div>
        <div>
          <button
            name="registerForm_register_button"
            onClick={this.handleRegister}
          >
            Register
          </button>
        </div>
      </form>
    );
  }
}

export default RegisterForm;
