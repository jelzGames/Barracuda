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
  constructor(props){
    super(props);
    this.state = {
      isloading: false
    }
  }
  
  logout = async() => {
    this.setState({
      isloading: true
    })
    let Logout = this.props.actions.Logout();
    let RemoveRefreshToken = this.props.actions.RemoveRefreshToken();
    await Promise.all([Logout, RemoveRefreshToken])
      .then((result) => {
      })
      .catch((error) => {
      })
      .finally(() => {
        this.props.actions.clean();
        this.props.postMessage(constants.BarracudaSesion);
      })
    this.setState({
      isloading: false
    })
  }
  
  render() {
    const { classes } = this.props;
    const { logout } = this;
    const { isloading } = this.state;
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
      RemoveRefreshToken: bindActionCreators(usersAction.RemoveRefreshToken, dispatch),
      clean: bindActionCreators(usersAction.clean, dispatch)
    }
  };
};

export default compose(
  PostMessageHoc,
  withRouter,
  withStyles(useStyles, { withTheme: true }),
  connect(mapStateToProps, mapDispatchToProps)
  )(LogoutButton);