import { Button } from '@material-ui/core';
import React, { Fragment } from 'react';
import ArrowBackIcon from '@material-ui/icons/ArrowBack';
import { withStyles } from '@material-ui/styles';
import OutlinedInput from '@material-ui/core/OutlinedInput';
import SaveIcon from '@material-ui/icons/Save';
import CloseIcon from '@material-ui/icons/Close';
import * as usersApi from "../../api/usersApi";
import * as constant from "../../constants";
import CustomTextField from "../common/customTextField";
import CustomSelect from "../common/customSelect";
import CustomButton from "../common/customButton";
import CustomHeader from "../common/customHeader";
import { withRouter } from 'react-router-dom';
import { compose } from 'redux';


const useStyles = theme => ({
    root: {
      '& > *': {
        margin: theme.spacing(1),
      },
    },
  });

  

export class AddUsers extends React.Component {
    constructor(props){
        super(props);
        this.state = {
            id: this.props.match.params.id,
            userid: this.props.match.params.id === constant.add ? "" : this.props.match.params.id,
            name: '',
            username: '',
            price: 0,
            country: "",
            countryIdx: "",
            items: [
                {
                    id: "USA",
                    name: "America"
                },
                {
                    id: "MX",
                    name: "Mexico"
                },
            ],
            validations: {
                "required": {
                    validation: () =>  { return this.state.userid.trim() === ""},
                    errorMessage: "Required"
                },
                "userId": {
                    validation: () => { return this.state.userid === "2" },
                    errorMessage: "checkid"
                },
                "userIdNotZero": {
                    validation: () => { return this.state.userid === "2" && this.state.userid !== "0"},
                    errorMessage: "checkidnotzero"
                },
                "requiredCountry": {
                    validation: () =>  { return this.state.country.trim() === ""},
                    errorMessage: "Required"
                },
            }, 
        }
    }


    componentDidMount(){
        
        if (this.state.id !== constant.add) {
            usersApi.Get(this.state.userid)
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
        }   
    }

    handleChange = (e) => {
        const {id, value} = e.target;
        this.setState({
            [id]: value
        })
    } 

    handleSave = () => {
        var model = {
            "name": this.state.name,
            "username": this.state.username
        }
        if (this.state.id === constant.add) {
            usersApi.Create(model)
            .then((result) => {
            })
            .catch((error) => {
                console.log(error)
            })
        }
        else {
            model.id = this.state.userid
            usersApi.Update(model)
            .then((result) => {
            })
            .catch((error) => {
                console.log(error)
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

    handleChangeLocation = (idx, idxState, element) => {
        this.setState({
            [idxState]: idx
        }, () => {
            this.handleChange(element)
        });
    }

    render() {
        const { handleChange, handleSave, validaData, handleChangeLocation} = this;
        const { classes } = this.props;
        const { name, username, userid, validations, countryIdx, items, price } = this.state;
        return(
            <Fragment>
            <div>
                <CustomHeader size={1} content={this.state.id === constant.add ? "Add" : "Edit"}/>
                <Button onClick={() => this.props.history.goBack()} color="primary">
                    <ArrowBackIcon fontSize="large"/>
                </Button>
                <form className={classes.root} > 
                        <CustomHeader size={2} content={"ID"}/>

                        <CustomTextField 
                            id={"userid"} 
                            value={userid} 
                            label={"Id"} 
                            autoFocus
                            handleChange={handleChange} 
                            errorConditions={
                                <Fragment>
                                    {validations["required"].validation() && validations["required"].errorMessage.concat("\n")} 
                                    {validations["userId"].validation() && validations["userId"].errorMessage.concat("\n")} 
                                    {validations["userIdNotZero"].validation() && validations["userIdNotZero"].errorMessage.concat("\n")} 
                                </Fragment>
                            } 
                        />

                        <CustomSelect 
                            id={"select-id"} 
                            labelId={"label-id"} 
                            label={"Country"}
                            idx={countryIdx}
                            idxState={"countryIdx"}
                            fieldToShow={"name"}
                            idItem={"country"}
                            items={items}
                            handleChange={handleChangeLocation}
                            errorConditions={
                                <Fragment>
                                    {validations["requiredCountry"].validation() && validations["requiredCountry"].errorMessage.concat("\n")} 
                                </Fragment>
                            } 
                        />

                        <h2>Name:</h2>
                        <OutlinedInput  id={"name"} value={name} type="text" onChange={handleChange}/>
                        <h2>Username:</h2>
                        <OutlinedInput  id={"username"} value={username} onChange={handleChange}/><br/>

                        <CustomTextField 
                            id={"price"} 
                            value={price} 
                            label={"Price"} 
                            isNumber="number"
                            maxDecimals={6}
                            handleChange={handleChange} 
                        />

                        <CustomButton 
                            disabled={validaData()}
                            color="primary" 
                            startIcon={<SaveIcon/>} 
                            handleClick={handleSave}
                            content={"Save"}
                        />
                    
                        <Button color="secondary" startIcon={<CloseIcon/>} variant="contained" size="large" onClick={() => this.props.history.goBack()}>
                            Cancel
                        </Button>
                </form>
            </div>

            </Fragment>
        )
    }
}

export default compose(
    withRouter,
    withStyles(useStyles, { withTheme: true }),
    )(AddUsers);