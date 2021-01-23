import React from "react";
import * as usersAction from "../../redux/actions/userAuthActions";
import { withStyles } from "@material-ui/styles";
import { withRouter } from "react-router-dom";
import { bindActionCreators, compose } from "redux";
import { connect } from "react-redux";

const useStyles = (theme) => ({

});

export class LogoutButton extends React.Component{
  
  logout = () => {
    this.props.actions.Logout();
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
      Logout: bindActionCreators(usersAction.Logout, dispatch)
    }
  };
};

export default compose(
  withRouter,
  withStyles(useStyles, { withTheme: true }),
  connect(mapStateToProps, mapDispatchToProps)
  )(LogoutButton);