import React from 'react';
import { withStyles } from '@material-ui/styles';
import { withTranslation } from "react-i18next";
import { compose, bindActionCreators } from "redux";
import { Grid } from '@material-ui/core';
import CustomTextField from "./customTextField";
import CustomButton from "./customButton";
import { connect } from "react-redux";
import * as usersAction from "../../redux/actions/userAuthActions";

const useStyles = theme => ({
    
});

export class CustomChangePassword extends React.Component {
    constructor(props){
        super(props);
        this.state = {
            newPassword:"",
            validations: {
                "newPassword": {
                    validation: () =>  { return this.state.newPassword.trim() === ""},
                    errorMessage: "Required"
                }
            }
        }
    }

    handleChange = (e) =>{
        const {id, value} = e.target;
        this.setState({
            [id]: value
        })     
    } 

    handlePassword = async() => {
        const { token } = this.props;
        this.setState({
          isloading: true
      })
      var model = {
          id: this.props.userAuth.id,
          email: this.props.userAuth.email,
          password: this.state.newPassword
      }
      await this.props.actions.ChangePassword(model, token)
      .then((result) => {
          if(token) {
              this.props.history.push("/");
          }
      })
      .catch((error) => {
        console.log(error)
      })
      .finally(() => {
          this.setState({
              isloading: false,
          });
      });
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

    render() {
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
                    handleChange={handleChange}
                    isNumber={"password"}
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
    withTranslation("common"),
    withStyles(useStyles, { withTheme: true }),
    connect(mapStateToProps, mapDispatchToProps)
)(CustomChangePassword);
