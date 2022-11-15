import React from "react";

class GameForm extends React.Component {
  constructor(props) {
    super(props);

    this.handleChangeToRegister = this.handleChangeToRegister.bind(this);
    this.handleInputChange = this.handleInputChange.bind(this);
    this.handleLogout = this.handleLogout.bind(this);
  }

  handleLogout(event) {
    event.preventDefault();
    this.props.onLogout();
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
    console.log(this.props)
    return (
      <form>
        <div>
          <h2>Game</h2>
        </div>
        <div>
          <hr/>
          <ul>
            {(this.props.players || []).map((player) => (
              <li key={player.id}>
                ID={player.id} | LOGIN={player.login}
              </li>
            ))}
          </ul>
          <hr/>
        </div>
        <div>
          <button name="game_logout_button" onClick={this.handleLogout}>
            Logout
          </button>
        </div>
      </form>
    );
  }
}

export default GameForm;
