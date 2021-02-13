import React from "react";
import * as usersAction from "../../redux/actions/userAuthActions";
import { withStyles } from "@material-ui/styles";
import { withRouter } from "react-router-dom";
import { bindActionCreators, compose } from "redux";
import { connect } from "react-redux";
import PostMessageHoc from "../../helpers/postMessageHelper";
import * as constants from "../../constants";
import { Fragment } from "react";
import CustomSpinner from "../common/customSpinner";

const useStyles = (theme) => ({
    spinnerPaper: {
      backgroundColor: "transparent",
      padding: theme.spacing(0, 0, 0),
      border: 'none'
  },
});

export class LogoutButton extends React.Component{
  
  logout = async() => {
    this.setState({
      isloading: true
    })
    let Logout = this.props.actions.Logout();
    let RemoveRefreshToken = this.props.actions.RemoveRefreshToken();
    await Promise.all([Logout, RemoveRefreshToken])
      .then((result) => {
        this.props.postMessage(constants.BarracudaSesion);
      })
      .catch((error) => {
        console.log(error)
    });
    this.setState({
      isloading: false
    })
  }
  
  render() {
    const { classes } = this.props;
    const { logout, isloading } = this;
    return (
      <Fragment>
        {isloading && 
          <CustomSpinner open={true} paperClass={classes.spinnerPaper} />
        }
        <button onClick={logout}>
          Log Out
        </button>  
      </Fragment>
      
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
  PostMessageHoc,
  withRouter,
  withStyles(useStyles, { withTheme: true }),
  connect(mapStateToProps, mapDispatchToProps)
  )(LogoutButton);