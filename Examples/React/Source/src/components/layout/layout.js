import React, { Fragment } from 'react';
import CssBaseline from '@material-ui/core/CssBaseline';
import TopBar from './topbar';
import NavMenu from './navMenu';
import MainContainer from './mainContainer';
import { withStyles} from "@material-ui/core/styles";
import { bindActionCreators, compose } from 'redux';
import { connect } from "react-redux";
import { withRouter } from 'react-router-dom';
import * as usersAction from "../../redux/actions/userAuthActions";
import PostMessageHoc from "../../helpers/postMessageHelper";

const useStyles = theme => ({
  root: {
    display: "flex"
  },
});

export class Layout extends React.Component {
  constructor(props) {
    super(props);
    this.state= {
      open: false
    };
  }

  componentDidMount() {
     this.props.actions.GetUserAuth();
  }

  componentDidUpdate(prevState){
     console.log(this.props.communicationState)
    if(prevState.communicationState !== this.props.communicationState){
     
    }
  }
 
  handleDrawerOpen = () => {
    this.setState({
      open: true
    })
  };
 
  handleDrawerClose = () => {
    this.setState({
      open: false
    })
  };
 
  render() {
    const { classes } = this.props;
    const { open } = this.state;
    const { handleDrawerOpen,  handleDrawerClose } = this;
    return (
      <div className={classes.root}>
        {this.props.userAuth && this.props.userAuth.id && this.props.userAuth.validEmail ? (
          <Fragment>
            <CssBaseline />
            <TopBar open={open} handleDrawerOpen={handleDrawerOpen} />
            <NavMenu open={open} handleDrawerClose={handleDrawerClose} handleDrawerOpen={handleDrawerOpen}/>
            <MainContainer children={this.props.children}/>
          </Fragment>  
        ):(
          <div>
            <MainContainer children={this.props.children}/>
          </div>
        )}
        
      </div>
    );
  }
}

const mapStateToProps = (state) => {
  return {
      userAuth: state.userAuth
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
      actions: {
        GetUserAuth: bindActionCreators(usersAction.GetUserAuth, dispatch)
      }
  };
};

export default compose(
  withRouter,
  withStyles(useStyles, { withTheme: true }),
  connect(mapStateToProps, mapDispatchToProps)
  )(Layout);