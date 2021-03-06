import React from 'react';
import { Fragment } from 'react';
import { withRouter } from 'react-router-dom';
import { bindActionCreators, compose } from 'redux';
import CustomChangePassword from '../components/common/customChangePassword';
import CustomModal from './common/customModal';
import * as usersAction from "../redux/actions/userAuthActions";
import { connect } from "react-redux";
import * as constants from "../constants";
import { Grid } from '@material-ui/core';
import { CustomButton } from './common/customButton';
import CustomSpinner from "../components/common/customSpinner";
import { withStyles } from '@material-ui/styles';

const useStyles = theme => ({
    spinnerPaper: {
        backgroundColor: "transparent",
        padding: theme.spacing(0, 0, 0),
        border: 'none'
    }
});

export class verifyTokens extends React.Component {
    constructor(props){
        super(props);
        const params = new URLSearchParams(window.location.search);
        this.state = {
            changePasswordToken: params.get("changePasswordToken"),
            validEmailToken: params.get("validEmailToken"),
            message: "This email has expired.",
            hiddenButton: false
        }
    }

    componentDidMount(){
        if(this.state.validEmailToken){
            this.handleValidEmailToken()
        }
    }

    handleResendValidEmail = async() => {
        const { validEmailToken} = this.state;
        this.setState({
          isloading: true
        })
        await this.props.actions.ResendValidEmail(validEmailToken)
        .then((result) => {
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

    handleResendModal = async() => {
        this.setState({
            hiddenButton: true
        })
        await this.handleResendValidEmail();
        this.setState({
            message: "A new email has been sent! please check your email inbox and junk."
        })
    }

    bodyModalResendEmail = () => {
        const { handleResendModal } = this;
        return(
            <Grid container >
                <Grid item xs={12} >
                    <h3>
                        {this.state.message}
                    </h3>
                </Grid>
                <Grid item xs={12} >
                    <CustomButton
                    disabled={this.state.hiddenButton}
                    content={"Accept"}
                    color={"primary"}
                    handleClick={handleResendModal}
                    />
                </Grid>
            </Grid>    
        )
    }

    handleValidEmailToken = async() => {
        const { validEmailToken } = this.state;
        this.setState({
          isloading: true
        })
        await this.props.actions.ValidEmail(validEmailToken)
        .then((result) => {
            this.props.history.push("/");
        })
        .catch((error) => {
            if(error === constants.ValidateTokenConfirmEmailExpired){
                this.setState({
                    openModal: true
                })
            }
            else{
                alert(error)
            }    
        })
        .finally(() => {
            this.setState({
                isloading: false,
            });

        });
    }

    render() {
        const { changePasswordToken, validEmailToken, isloading } = this.state;
        const { classes } = this.props;
        return(
            <Fragment>
                {!validEmailToken && changePasswordToken ? (
                    <CustomModal modal={true}
                        item={<CustomChangePassword token={changePasswordToken} history={this.props.history} />}
                    />  
                ) : (
                    !validEmailToken &&
                    this.props.history.push("/")
                )            
                }
                <CustomModal modal={this.state.openModal}
                    item={this.bodyModalResendEmail()}
                />
                {isloading && 
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
            ValidEmail: bindActionCreators(usersAction.ValidEmail, dispatch),
            ResendValidEmail: bindActionCreators(usersAction.ResendValidEmail, dispatch)
        }
    };
};

export default compose(
    withRouter,
    withStyles(useStyles, { withTheme: true }),
    connect(mapStateToProps, mapDispatchToProps)
    )(verifyTokens);
