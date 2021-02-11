import { Button } from '@material-ui/core';
import React, { Fragment } from 'react';
import ArrowBackIcon from '@material-ui/icons/ArrowBack';
import { withStyles } from '@material-ui/styles';
import SaveIcon from '@material-ui/icons/Save';
import CloseIcon from '@material-ui/icons/Close';
import * as usersApi from "../../api/usersApi";
import * as constant from "../../constants";
import CustomTextField from "../common/customTextField";
import CustomButton from "../common/customButton";
import CustomHeader from "../common/customHeader";
import { withRouter } from 'react-router-dom';
import { compose } from 'redux';
import * as usersAuthApi from "../../api/usersAuthApi";
import CustomSpinner from '../common/customSpinner';
import { v4 as uuidv4 } from 'uuid';

const useStyles = theme => ({
    root: {
      '& > *': {
        margin: theme.spacing(1),
      },
    },
    spinnerPaper: {
        backgroundColor: "transparent",
        padding: theme.spacing(0, 0, 0),
        border: 'none'
    }
  });

export class AddUsers extends React.Component {
    constructor(props){
        super(props);
        this.state = {
            id: this.props.match.params.id,
            userid: this.props.match.params.id === constant.add ? "" : this.props.match.params.id,
            email: "",
            name: '',
            username: '',
            newPassword: "",
            isloading: false,
            tenants: "",
            scopes: "",
            validations: {
                "password": {
                    validation: () =>  { return this.state.id === constant.add ? this.state.newPassword.trim() === "" : false},
                    errorMessage: "Required"
                },
                "email": {
                    validation: () =>  { return this.state.id === constant.add ? this.state.email.trim() === "" : false},
                    errorMessage: "Required"
                }
            }, 
        }
    }


    componentDidMount(){
        this.loadUser();
    }

    loadUser = async() => {
        if (this.state.id !== constant.add) {
            this.setState({
                isloading: true
            });
            await usersApi.Get(this.state.userid)
            .then((result) => {
                this.setState({
                    userid: result.id,
                    name: result.name,
                    username: result.username
                })
            })
            .catch((error) => {
                console.log(error)
            }) 
            await usersAuthApi.GetAdditional(this.state.userid)
            .then((result) => {
                this.setState({
                    scopes: result.scopes,
                    tenants: result.tenants
                })
            })
            .catch((error) => {
                console.log(error)
            }) 
            this.setState({
                isloading: false
            });
        } 
    }

    handleChange = (e) => {
        const {id, value} = e.target;
        this.setState({
            [id]: value
        })
    } 

    handleSave = async() => {
        this.setState({
            isloading: true
        });
        var id = uuidv4();
        var model = {
            "id": id,
            "name": this.state.name,
            "username": this.state.username,
            "email": this.state.email,
            "tenants": this.state.tenants,
            "scopes": this.state.scopes
        }
        if (this.state.id === constant.add) {
            var flag = false;
            await usersAuthApi.CheckEmail(this.state.email)
                .then((result) => {
                    flag = true;
                })
                .catch((error) => {
                    if(error === constant.Found ){
                        alert("The email already exist");
                    }
                    else{
                        console.log(error)
                    }
                })
            if(flag){
                await usersApi.CheckUsername(this.state.username)
                .then((result) => {
                })
                .catch((error) => {
                    if(error === constant.Found ){
                        flag = false;
                        alert("The username already exist");
                    }
                    else{
                        flag = false;
                        console.log(error)
                    }
                })
            }
            if(flag){
                await usersApi.Create(model)
                .then((result) => {
                })
                .catch((error) => {
                    flag = false
                    console.log(error)
                })
                if(flag){
                    var Authmodel = {
                        id: id,
                        email: this.state.email,
                        password: this.state.newPassword
                    }
                    await usersAuthApi.AddUser(Authmodel)
                    .then((result) => {
                    })
                    .catch((error) => {
                        flag = false;
                        console.log(error)
                    })
                }
            }
            this.Additional(flag, id);
        }
        else {
            model.id = this.state.userid;
            await usersApi.Update(model)
            .then((result) => {
            })
            .catch((error) => {
                console.log(error)
            }) 

            this.Additional(true, model.id);
        }
        this.setState({
            isloading: false
        });
    }

    Additional = async(flag, id) => {
        if(flag){
            var modelScope = {
                id: id,
                scopes: this.state.scopes.split(",")
            }
            console.log(modelScope)
            await usersAuthApi.UpdateScopes(modelScope)
            .then((result) => {
            })
            .catch((error) => {
                flag = false;
                console.log(error);
            })
        }
        if(flag){
            var modelTenants = {
                id: id,
                tenants: this.state.tenants.split(",")
            }
            console.log(modelTenants)
            await usersAuthApi.UpdateTenants(modelTenants)
            .then((result) => {
                if (this.state.id === constant.add) {
                    alert("The user has been created");
                }
                else{
                    alert("The user has been updated");
                }
            })
            .catch((error) => {
                flag = false;
                console.log(error);
            })
        }
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
        const { handleChange, handleSave, validaData} = this;
        const { classes } = this.props;
        const { name, username, userid, validations, newPassword, isloading, email, tenants, scopes } = this.state;
        return(
            <Fragment>
                {isloading && 
                    <CustomSpinner open={true} paperClass={classes.spinnerPaper} />
                }
                <CustomHeader size={1} content={this.state.id === constant.add ? "Add" : "Edit"}/>
                <Button onClick={() => this.props.history.goBack()} color="primary">
                    <ArrowBackIcon fontSize="large"/>
                </Button>
                    <CustomHeader size={2} content={"ID"}/>
                    <CustomTextField 
                        id={"userid"} 
                        value={userid} 
                        label={"Id"} 
                        autoFocus
                        handleChange={handleChange}
                        disabled={true}
                    />
                    {this.state.id === constant.add &&
                        <Fragment>
                            <CustomTextField 
                                id={"newPassword"} 
                                value={newPassword} 
                                label={"password"}  
                                handleChange={handleChange}
                                isNumber={"password"}
                                errorConditions={
                                    <Fragment>
                                        {validations["password"].validation() && validations["password"].errorMessage.concat("\n")} 
                                    </Fragment>
                                } 
                            />
                            <CustomTextField 
                            id={"email"} 
                            value={email} 
                            label={"email"}  
                            handleChange={handleChange}
                            errorConditions={
                                <Fragment>
                                    {validations["email"].validation() && validations["email"].errorMessage.concat("\n")} 
                                </Fragment>
                            }  
                            />
                        </Fragment>
                    }
                        <CustomTextField 
                            id={"name"} 
                            value={name} 
                            label={"Name"} 
                            handleChange={handleChange}
                        />

                        <CustomTextField 
                            id={"username"} 
                            value={username} 
                            label={"Username"} 
                            handleChange={handleChange}
                        />

                        <CustomTextField 
                            id={"tenants"} 
                            value={tenants} 
                            label={"Tenants"} 
                            handleChange={handleChange}
                        />

                        <CustomTextField 
                            id={"scopes"} 
                            value={scopes} 
                            label={"Scopes"} 
                            handleChange={handleChange}
                        />

                        <CustomButton 
                            disabled={validaData()}
                            color="primary" 
                            startIcon={<SaveIcon/>} 
                            handleClick={handleSave}
                        />{" "}
                        <CustomButton 
                            color="secondary" 
                            startIcon={<CloseIcon/>} 
                            handleClick={() => this.props.history.goBack()}
                        />
            </Fragment>
        )
    }
}

export default compose(
    withRouter,
    withStyles(useStyles, { withTheme: true }),
    )(AddUsers);