import React from "react"
import { withStyles } from "@material-ui/styles";
import { Container, Grid } from "@material-ui/core";
import CustomTextField from "../common/customTextField";
import CustomButton from "../common/customButton";
import { withRouter } from "react-router-dom";
import { bindActionCreators, compose } from "redux";
import { connect } from "react-redux";
import { Fragment } from "react";
import * as usersAction from "../../redux/actions/userAuthActions";
import {GoogleLogin } from 'react-google-login';
import CustomSpinner from "../common/customSpinner";
import CustomIcon from "../common/customIcon";
import * as icons from "../libraries/icons";
import FacebookLogin from 'react-facebook-login';
import { LinkedIn } from 'react-linkedin-login-oauth2';
import MicrosoftLogin from "react-microsoft-login";
import CustomImage from "../common/customImage";
import CustomLink from "../common/customLink";
import env from "../../utils/env"; 

const useStyles = (theme) => ({
    closeIcon: {
        textAlign: "right"
      },
      newUserModalPaper: {
        padding: theme.spacing(0, 0, 0)
    },
    spinnerPaper: {
        backgroundColor: "transparent",
        padding: theme.spacing(0, 0, 0),
        border: 'none'
    },
    modal: {
        display: 'flex',
        alignItems: 'center'
    },
    buttonFacebook: {
        fontSize: "24px",
        width: "60px",
        height: "60px",
        color: "white",
        border:"0px transparent",
        backgroundColor: "#4968ad",
        borderRadius: "100%"
    },
    buttonLinkedIn: {
        width: "60px",
        height: "60px",
        border:"0px",
        borderRadius: "100%",
        backgroundColor: "#4968ad",
    },
    buttonGoogle: {
        width: "60px",
        height: "60px",
        border: "none",
        borderRadius: "100%",
        backgroundColor: "#FFFFFF", 
    },
    buttonMicrosoft: {
        width: "60px",
        height: "60px",
        border: "none",
        borderRadius: "100%",
        backgroundColor: "#FFFFFF",
        alignItems: 'center',
        justifyContent: 'center'
    },
    preview: {
        display: 'inline-flex',
        borderRadius: 2,
        border: '0px solid #eaeaea',
        marginBottom: 0,
        marginRight: 0,
        width: 24,
        height: 24,
        padding: 0,
        boxSizing: 'border-box'
      },
      previewInner: {
        display: 'flex',
        minWidth: 0,
        overflow: 'hidden'
      },
      img: {
        display: 'flex',
        width: 'auto',
        height: '100%'
      },
      previewCompany: {
        display: 'inline-flex',
        borderRadius: 2,
        border: '0px solid #eaeaea',
        marginBottom: 0,
        marginRight: 0,
        width: 100,
        height: 100,
        padding: 0,
        boxSizing: 'border-box'
      },
      previewInnerCompany: {
        display: 'flex',
        minWidth: 0,
        overflow: 'hidden'
      },
      imgCompany: {
        display: 'flex',
        width: 'auto',
        height: '100%'
      }
})

export class CustomLogin extends React.Component{
    constructor(props){
        super(props);
        this.state = {
            renderPassword: false,
            renderLogIn: true,
            renderSignUp: false,
            tabValue: 0,
            recoverPassword: "",
            loginEmail: "",
            loginPassword: "",
            registerEmail: "",
            confirmEmail: "",
            registerPassword: "",
            confirmPassword: "",
            isloading: false,
            validationsLogin: {
                "loginEmail": {
                    validation: () =>  { return this.state.loginEmail.trim() === ""},
                    errorMessage: "Required"
                },
                "loginPassword": {
                    validation: () =>  { return this.state.loginPassword.trim() === ""},
                    errorMessage: "Required"
                }
            },
            validationsSignUp: {
                "requiredEmail": {
                    validation: () =>  { return this.state.registerEmail.trim() === ""},
                    errorMessage: "Required"
                },
                "registerPassword": {
                    validation: () =>  { return this.state.registerPassword.trim() === ""},
                    errorMessage: "Required"
                }
            },
            validationsPassword: {
                "sendEmail": {
                    validation: () =>  { return this.state.recoverPassword.trim() === ""},
                    errorMessage: "Required"
                }
            }
        }
    }

    validaDataSignUp = () => {
        var temp = {...this.state.validationsSignUp};
        var flag = false;
        Object.keys(temp).forEach(function(key,index) {
            if (temp[key].validation()) {
                flag = true;
                return;
            }
        });
        return flag;
    }

    validaDataLogin = () => {
        var temp = {...this.state.validationsLogin};
        var flag = false;
        Object.keys(temp).forEach(function(key,index) {
            if (temp[key].validation()) {
                flag = true;
                return;
            }
        });
        return flag;
    }

    validaDataPassword = () => {
        var temp = {...this.state.validationsPassword};
        var flag = false;
        Object.keys(temp).forEach(function(key,index) {
            if (temp[key].validation()) {
                flag = true;
                return;
            }
        });
        return flag;
    }

    handlePassword = () => {
        this.setState({
            renderPassword: true,
            renderLogIn: false
        })
    }

    handleSignUp = () => {
        this.setState({
            renderSignUp: true,
            renderLogIn: false
        })
    }

    handleCloseForm = () => {
        this.setState({
            renderSignUp: false,
            renderLogIn: true,
            renderPassword: false
        })
    }

    handleValidEmailToken = async() => {
        const { loginEmail} = this.state;
        this.setState({
          isloading: true
        })
        await this.props.actions.ValidEmail(null, true, loginEmail)
        .then((result) => {
            this.props.history.push("/");
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

    handleLogIn = async() => {
        this.setState({
            isloading: true
        })
        var model = {
            email: this.state.loginEmail,
            password: this.state.loginPassword
        }
        await this.props.actions.LogIn(model)
        .then((result) => {
            this.props.history.push("/");
        })
        .catch( async(error) => {
            if(error = "NotValidEmailConfirmation"){
                await this.handleValidEmailToken();
                alert("The Email hasn't been confirmed");
            }
        })
        .finally(() => {
            this.setState({
                isloading: false,
            });
        });
    }

    handleRegister = async() => {
        this.setState({
            isloading: true
        })
        var model = {
            email: this.state.registerEmail,
            password: this.state.registerPassword
        }
        await this.props.actions.Register(model)
        .then((result) => {
            this.setState({
                tabValue: 0
            })
        })
        .catch((error) => {
        })
        .finally(() => {
            this.setState({
                isloading: false
            })
        })
    }

    handleChangeTab = (event, newValue) => {        
        this.setState({
            tabValue: newValue,
        })
    }

    handleChange = (e) =>{
        const {id, value} = e.target;
        this.setState({
            [id]: value
        })     
    } 

    successGoogle = async(response) => {
        if (response && response.tokenId) {
            await this.props.actions.SocialGoogle(response)
            .then( (result) => {
                this.props.history.push("/"); 
            })
            .catch((error) => {
            });
        }
    }

    failureGoogle = (error) => {
        console.log(error);     
    }

    failureFacebook = (error) => {
        console.log(error)
    }

    responseFacebook = async(response) => {
        if (response && response.id) {
            await this.props.actions.SocialFacebook(response)
            .then( (result) => {
                this.props.history.push("/"); 
            })
            .catch((error) => {
            });
        }
    }

    failureLinkedIn = (error) => {
        console.log(error)
    }

    successLinkedIn = (data) => {
        this.props.history.push("/"); 
    }

    responseMicrosoft = async(error, data, msall) => {
        if (data && data.account.accountIdentifier) {
            await this.props.actions.SocialMicrosoft(data)
            .then( (result) => {
            })
            .catch((error) => { 
            });
        }
    }

    handleForgotPassword = async() => {
        var model  ={
            email: this.state.recoverPassword
        }
        await this.props.actions.ForgotPassword(model)
        .then( (result) => {
        })
        .catch((error) => { 
        });
    }

    renderPassword = () => {   
        const { classes } = this.props;
        const { handleCloseForm, handleChange, validaDataPassword, handleForgotPassword } = this;
        const { recoverPassword } = this.state;
        
        return(
            <Fragment>
                <div className={classes.closeIcon} >
                    <CustomIcon 
                        Icon={icons.icon.CloseIcon}
                        fontSize={"large"}
                        handleClick={handleCloseForm}
                    />
                </div>
                <Grid container className={classes.bodyPassword} >
                    <Grid item xs={12}>
                        <CustomTextField 
                            id={"recoverPassword"} 
                            value={recoverPassword} 
                            label={"email"}  
                            handleChange={handleChange}
                        />
                    </Grid>
                    <Grid item xs={12} style={{padding: "24px"}}>
                        <CustomButton 
                            disabled={validaDataPassword()}
                            color="primary"
                            handleClick={handleForgotPassword}
                            content={"Recover"}
                            fullWidth={true}
                        />
                    </Grid>
                    
                </Grid>
            </Fragment>
        ) 
    }

    renderLogin = () => {

        const { loginEmail, loginPassword} = this.state;
        const { handleChange, handleLogIn, failureGoogle, successGoogle, validaDataLogin, handleSignUp,
                handlePassword, responseFacebook, failureLinkedIn, successLinkedIn, responseMicrosoft
        } = this;
        const { classes } = this.props;

        return(
            <Fragment>
                <Grid container direction="column" style={{minWidth:264}}>
                    <Grid item xs={12}>
                        <CustomImage 
                            url={"../../../../internetLogo.svg"} 
                            topPreview={classes.previewCompany}
                            bottomPreview={classes.previewInnerCompany}
                            image={classes.imgCompany}
                        />
                    </Grid>
                    
                    <Grid item xs={12}>
                        <CustomTextField 
                            id={"loginEmail"} 
                            value={loginEmail} 
                            label={"email"}  
                            handleChange={handleChange}
                        />
                    </Grid>
                    <Grid item xs={12} >
                        <CustomTextField 
                            id={"loginPassword"} 
                            value={loginPassword} 
                            label={"password"}  
                            handleChange={handleChange}
                            isNumber={"password"}
                        />
                    </Grid>
                    
                    <Grid item xs={12} style={{padding:"24px"}}>
                        <CustomButton 
                            disabled={validaDataLogin()}
                            color="primary" 
                            handleClick={handleLogIn}
                            content={"Login"}
                            fullWidth={true}
                        />  
                    </Grid>
                </Grid>
                <Grid container direction="row" justify="flex-end" alignItems="center" >
                    <Grid style={{paddingRight:"8px"}} >
                        <GoogleLogin
                            clientId={env.googleID}
                            onSuccess={successGoogle}
                            onFailure={failureGoogle}
                            cookiePolicy={'single_host_origin'}
                            render={renderProps => (
                                <button onClick={renderProps.onClick} className={classes.buttonGoogle}>
                                    <CustomImage 
                                        url={"../../../../GoogleLogo.svg"} 
                                        topPreview={classes.preview}
                                        bottomPreview={classes.previewInner}
                                        image={classes.img}
                                    /> 
                                </button>
                            )}
                        />
                    </Grid>
                    <Grid style={{paddingRight:"8px"}}>
                        <FacebookLogin
                            appId={env.facebookID}
                            autoLoad={false}
                            fields="name,email,picture"
                            callback={responseFacebook}
                            icon="fa-facebook"
                            textButton={null}
                            cssClass={classes.buttonFacebook}
                        />
                    </Grid>
                    {/* <Grid style={{paddingRight:"8px"}}>
                        <LinkedIn
                            clientId="78nsm236v91m73"
                            onFailure={failureLinkedIn}
                            onSuccess={successLinkedIn}
                            redirectUri="https://www.mysite.dev:3000"
                            className={classes.buttonLinkedIn}
                        >
                            <CustomImage 
                                    url={"../../../../linkedinLogo.svg"} 
                                    topPreview={classes.preview}
                                    bottomPreview={classes.previewInner}
                                    image={classes.img}
                                />
                        </LinkedIn>
                    </Grid> */}
                    <Grid>
                        <MicrosoftLogin
                            clientId={env.microsoftID}
                            buttonTheme="dark_short"
                            authCallback={responseMicrosoft}
                            withUserData={true}
                            graphScopes={["user.readwrite.all", "directory.readwrite.all"]}
                            children={
                                <Grid container className={classes.buttonMicrosoft}>
                                    <CustomImage 
                                        url={"../../../../MicrosoftLogo.svg"} 
                                        topPreview={classes.preview}
                                        bottomPreview={classes.previewInner}
                                        image={classes.img}
                                    />
                                </Grid>   
                            }
                        />
                    </Grid>
                </Grid>
                <Grid container direction="row" justify="space-between" alignItems="center" style={{paddingTop: "24px"}} >
                    <Grid>
                        <CustomLink handleClick={handleSignUp} content={"SignUp"}/>
                    </Grid>

                    <Grid>
                        <CustomLink handleClick={handlePassword} content={"Forgotpassword?"}/>
                    </Grid>
                </Grid>
            </Fragment> 
        )
    }

    renderSignUp = () => {

        const { registerEmail, registerPassword} = this.state;
        const { handleChange, handleRegister, validaDataSignUp, handleCloseForm } = this;

        return(
            <Fragment>
                <Grid container justify="flex-end">
                    <CustomIcon 
                        Icon={icons.icon.CloseIcon}
                        fontSize={"large"}
                        handleClick={handleCloseForm}
                    />
                </Grid>
                <Grid container direction="column" style={{minWidth:264}}>
                    <Grid item xs={12}>
                        <CustomTextField 
                            id={"registerEmail"} 
                            value={registerEmail} 
                            label={"email"}  
                            handleChange={handleChange}
                        />
                    </Grid>
                    <Grid item xs={12}>
                        <CustomTextField 
                            id={"registerPassword"} 
                            value={registerPassword} 
                            label={"password"}  
                            handleChange={handleChange}
                            isNumber={"password"}
                        />
                    </Grid>
                    <Grid item xs={12} style={{padding:"24px"}}>
                        <CustomButton 
                            disabled={validaDataSignUp()}
                            color="primary" 
                            handleClick={handleRegister}
                            content={"SignUp"}
                            fullWidth={true}
                        /> 
                    </Grid>
                </Grid>
            </Fragment>
        )
    }

    renderForm = () => {
        const { renderSignUp, renderLogIn, renderPassword } = this.state;

        return(
            <Fragment>
                {renderLogIn && 
                    this.renderLogin()
                }
                {renderSignUp && 
                    this.renderSignUp()
                }
                {renderPassword &&
                    this.renderPassword()
                }
            </Fragment>
            
            
        )
    }

    render(){
        const { renderForm} = this;
        const { classes, userAuth } = this.props;
        const { isloading } = this.state;
        return(
            <Fragment>
                    {(!userAuth || !userAuth.id || !userAuth.validEmail) && 
                        <Container >
                            {renderForm()} 
                            {isloading && 
                                <CustomSpinner open={true} paperClass={classes.spinnerPaper} />
                            }
                        </Container>
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
            LogIn: bindActionCreators(usersAction.LogIn, dispatch),
            Register: bindActionCreators(usersAction.Register, dispatch),
            SocialGoogle: bindActionCreators(usersAction.SocialGoogle, dispatch),
            SocialFacebook: bindActionCreators(usersAction.SocialFacebook, dispatch),
            SocialMicrosoft: bindActionCreators(usersAction.SocialMicrosoft, dispatch),
            ForgotPassword: bindActionCreators(usersAction.ForgotPassword, dispatch),
            ValidEmail: bindActionCreators(usersAction.ValidEmail, dispatch),
        }
    };
};

export default compose(
    withRouter,
    withStyles(useStyles, { withTheme: true }),
    connect(mapStateToProps, mapDispatchToProps)
    )(CustomLogin);
