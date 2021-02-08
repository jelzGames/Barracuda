import React from "react";
import * as usersAction from "../../redux/actions/userAuthActions";
import { withStyles } from "@material-ui/styles";
import { withRouter } from "react-router-dom";
import { bindActionCreators, compose } from "redux";
import { connect } from "react-redux";
import PostMessageHoc from "../../helpers/postMessageHelper";

const useStyles = (theme) => ({

});

export class LogoutButton extends React.Component{
  
  logout = () => {
    let Logout = this.props.actions.Logout();
    let RemoveRefreshToken = this.props.actions.RemoveRefreshToken();
    return Promise.all([Logout, RemoveRefreshToken])
      .then((result) => {
        this.props.postMessage("usersReactBarracuda")
      })
      .catch((error) => {
        console.log(error)
    });
  }
  
  render() {
    const { logout } = this;
    return (
      <button onClick={logout}>
        Log Out
      </button>
    );
  }
  
};

const mapStateToProps = (state) => {
  return {
      userAuth: state.userAuth
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    actions: {
      Logout: bindActionCreators(usersAction.Logout, dispatch),
      RemoveRefreshToken: bindActionCreators(usersAction.RemoveRefreshToken, dispatch)
    }
  };
};

export default compose(
  withRouter,
  withStyles(useStyles, { withTheme: true }),
  connect(mapStateToProps, mapDispatchToProps)
  )(LogoutButton);