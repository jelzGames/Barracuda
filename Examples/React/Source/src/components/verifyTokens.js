import React from 'react';
import { Fragment } from 'react';
import { withRouter } from 'react-router-dom';
import { bindActionCreators, compose } from 'redux';
import CustomChangePassword from '../components/common/customChangePassword';
import CustomModal from './common/customModal';
import * as usersAction from "../redux/actions/userAuthActions";
import { connect } from "react-redux";
import { Alert } from '@material-ui/lab';
import CustomButton from './common/customButton';


export class verifyTokens extends React.Component {
    constructor(props){
        super(props);
        const params = new URLSearchParams(window.location.search);
        this.state = {
            changePasswordToken: params.get("changePasswordToken"),
            validEmailToken: params.get("validEmailToken")
        }
    }

    componentDidMount(){
        if(this.state.validEmailToken){
            this.handleValidEmailToken()
        }
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
            console.log(error)
        })
        .finally(() => {
            this.setState({
                isloading: false,
            });

        });
    }

    renderValidEmailToken = () => {
        const { handleValidEmailToken } = this;
        return(
            <div>
                <CustomModal modal={true}
                    item={
                        <Alert
                        action={
                            <CustomButton
                                color="primary" 
                                handleClick={handleValidEmailToken}
                                content={"ok"}
                                fullWidth={true}
                            />
                        }
                        >
                            Your email has been registered with success press OK to accept and redirect
                        </Alert>
                    }
                />
            </div>   
        )
        
    }

    render() {
        const { changePasswordToken, validEmailToken } = this.state;
        return(
            <Fragment>
                {!validEmailToken && changePasswordToken ? (
                    <div>
                        <CustomModal modal={true}
                            item={<CustomChangePassword token={changePasswordToken} history={this.props.history} />}
                        />  
                    </div>
                ) : (
                    this.props.history.push("/")
                )            
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
        }
    };
};

export default compose(
    withRouter,
    connect(mapStateToProps, mapDispatchToProps)
    )(verifyTokens);
