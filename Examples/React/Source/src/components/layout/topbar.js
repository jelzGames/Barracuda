import React, { Fragment } from 'react';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import IconButton from '@material-ui/core/IconButton';
import MenuIcon from '@material-ui/icons/Menu';
import Typography from '@material-ui/core/Typography';
import clsx from 'clsx';
import { withStyles } from "@material-ui/core/styles";
import { AccountCircle } from '@material-ui/icons';
import Menu from '@material-ui/core/Menu';
import { Grid, MenuItem } from '@material-ui/core';
import { bindActionCreators, compose } from "redux";
import * as usersAction from "../../redux/actions/userAuthActions";
import LogoutButton from "../auth/logoutButton";
import { withRouter } from 'react-router-dom';
import { connect } from "react-redux";
import CustomModal from "../common/customModal";
import CustomTextField from "../common/customTextField";
import CustomButton from "../common/customButton";
import CustomSpinner from "../common/customSpinner";


const drawerWidth = 240;

const useStyles = theme => ({
  spinnerPaper: {
    backgroundColor: "transparent",
    padding: theme.spacing(0, 0, 0),
    border: 'none'
  },
  toolbar: {
    paddingRight: 24, 
  },
  appBar: {
    zIndex: theme.zIndex.drawer + 1,
    transition: theme.transitions.create(['width', 'margin'], {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.leavingScreen,
    }),
  },
  appBarShift: {
    marginLeft: drawerWidth,
    width: `calc(100% - ${drawerWidth}px)`,
    transition: theme.transitions.create(['width', 'margin'], {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.enteringScreen,
    }),
  },
  menuButton: {
    marginRight: 36,
  },
  menuButtonHidden: {
    display: 'none',
  },
  title: {
    flexGrow: 1,
  }
});


export class TopBar extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      isloading: false,
      anchorEl: null,
      modalPassword: false,
      newPassword: "",
      validations: {
        "newPassword": {
            validation: () =>  { return this.state.newPassword.trim() === ""},
            errorMessage: "Required"
        }
      }
    }; 
  }

  validaData = () => {
    var temp = {...this.state.validations};
    var flag = false;
    Object.keys(temp).forEach(function(key,index) {
        if (temp[key].validation()) {
            flag = true;
            return;
        }
    });
    return flag;
  }

  handleChange = (e) =>{
    const {id, value} = e.target;
    this.setState({
        [id]: value
    })     
  } 

  handleMenu = (event) => {
    this.setState({
      anchorEl: event.currentTarget
    })
  };

  handleClose = () => {
    this.setState({
      anchorEl: false
    })
  };

  handleModalPassword = () => {
    this.setState({
      anchorEl: false,
      modalPassword: true
    })  
  }

  handleCloseModal = () => {
    this.setState({
      modalPassword: false
    })
  }

  handlePassword = async() => {
    this.setState({
      isloading: true
  })
  var model = {
      id: this.props.userAuth.id,
      email: this.props.userAuth.email,
      password: this.state.newPassword
  }
  await this.props.actions.ChangePassword(model)
  .then((result) => {
    alert("The password has been changed");
    this.handleCloseModal();
  })
  .catch((error) => {
    alert(error)
  })
  .finally(() => {
      this.setState({
          isloading: false,
      });
  });
  }

  handleProfile = () => {
    this.setState({
      anchorEl: false
    })
    this.props.history.push(`/profile`)
  }

  renderModalPassword = () => {
    const { handleChange, handlePassword, validaData } = this;
    const { newPassword } = this.state;
    return(
      <Grid container direction="column" style={{minWidth:264}}>
        <Grid item xs={12} style={{padding:"24px"}}> 
          Type your new password
        </Grid>
        <Grid item xs={12}>
          <CustomTextField 
            id={"newPassword"} 
            value={newPassword} 
            label={"password"}
            isNumber={"password"}
            handleChange={handleChange}
          />
        </Grid>
        <Grid item xs={12} style={{padding:"24px"}}>
          <CustomButton 
              disabled={validaData()}
              color="primary" 
              handleClick={handlePassword}
              content={"Accept"}
              fullWidth={true}
          />  
        </Grid>
      </Grid>
      
    );
  }


  render() {
      const { classes, open, handleDrawerOpen } = this.props;   
      const {handleMenu, handleClose, handleModalPassword, renderModalPassword, handleCloseModal, handleProfile} = this;   
      const {anchorEl, modalPassword } = this.state;
      return (
          <Fragment>
            {this.props.error && 
            <div>Oops... {this.props.error.message}</div>          
            }
            {this.props.isLoading &&
              <div>loading...</div>
            }
          <AppBar position = "absolute" className={clsx(classes.appBar, open && classes.appBarShift)}>
              <Toolbar className={classes.toolbar}>
                  <IconButton
                      edge="start"
                      color="inherit"
                      aria-label="open drawer"
                      onClick={handleDrawerOpen}
                      className={clsx(classes.menuButton, open && classes.menuButtonHidden)}
                  >
                      <MenuIcon />
                  </IconButton>
                  
                  <Typography onClick={() => this.props.history.push("/")} style={{cursor: "pointer"}} component="h6" variant="h6" noWrap className={classes.title}>
                      Home
                  </Typography>
                  <IconButton
                    aria-label="account of current user"
                    aria-controls="menu-appbar"
                    aria-haspopup="true"
                    onClick = {handleMenu}
                    color="inherit"
                  >
                    <AccountCircle />
                  </IconButton>
                  <Menu
                    id="menu-appbar"
                    anchorEl={anchorEl}
                    anchorOrigin={{
                        vertical: 'top',
                        horizontal: 'right',
                    }}
                    keepMounted
                    transformOrigin={{
                        vertical: 'top',
                        horizontal: 'right',
                    }}
                    open={Boolean(anchorEl)}
                    onClose={handleClose}
                  >
                    {this.props.userAuth.id &&
                      <div>
                        <MenuItem onClick={handleProfile}>Profile</MenuItem>
                        <MenuItem onClick={handleClose}>My account</MenuItem>
                        <MenuItem onClick={handleModalPassword}>Change Password</MenuItem>
                        <MenuItem><LogoutButton></LogoutButton></MenuItem>
                      </div>
                    }
                  </Menu>
              </Toolbar>
          </AppBar>
            <CustomModal 
              modal={modalPassword} 
              item={renderModalPassword()} 
              handleCloseModal={handleCloseModal} 
            />
            {this.state.isloading && 
              <CustomSpinner open={true} paperClass={classes.spinnerPaper} />
            }
          </Fragment>
      )
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
      ChangePassword: bindActionCreators(usersAction.ChangePassword, dispatch)
  }
  };
};

export default compose(
  withRouter,
  withStyles(useStyles, { withTheme: true }),
  connect(mapStateToProps, mapDispatchToProps)
  )(TopBar);


    


