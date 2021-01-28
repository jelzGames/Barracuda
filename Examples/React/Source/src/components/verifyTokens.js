import React from 'react';
import { Fragment } from 'react';
import { withRouter } from 'react-router-dom';
import { bindActionCreators, compose } from 'redux';
import CustomChangePassword from '../components/common/customChangePassword';
import CustomModal from './common/customModal';
import * as usersAction from "../redux/actions/userAuthActions";
import { connect } from "react-redux";
import * as constants from "../constants";

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
        await this.props.actions.ValidEmail(validEmailToken, false, "")
        .then((result) => {
            this.props.history.push("/");
        })
        .catch((error) => {
            if(error === constants.ValidateTokenConfirmEmailExpired){
                console.log("error")
                console.log(error)
            }
            else{
                console.log(error)
            }
            
        })
        .finally(() => {
            this.setState({
                isloading: false,
            });

        });
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
