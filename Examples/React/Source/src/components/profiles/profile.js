import React, { Fragment } from "react";
import { compose } from "redux";
import { withStyles } from "@material-ui/styles";
import { Container, Grid } from "@material-ui/core";
import * as countries from "../libraries/localization/countries";
import * as states from "../libraries/localization/states/states";
import * as constants from "../../constants";
import * as usersApi from "../../api/usersApi";
import * as cities from "../libraries/localization/cities/mexico/mexico";
import CustomSelect from "../common/customSelect";
import CustomTextField from "../common/customTextField";
import CustomDragAndDrop from "../common/customDragAndDrop";
import { withRouter } from "react-router-dom";
import { connect } from "react-redux";
import CustomSwiper from "../common/customSwiper";
import userModel from "../../datatest/userModel";
import CustomModal from "../common/customModal";
import * as blobsApi from "../../api/blobsApi";
import CustomIcon from "../common/customIcon";
import CustomButton from "../common/customButton";
import CustomList from "../common/customList";
import * as icons from "../libraries/icons";
import { permissionFile, add } from "../../constants";
import CustomImage from "../common/customImage";
import CustomSpinner from "../common/customSpinner";
import { BottomBar } from "../layout/bottomBar";

const useStyles = theme => ({
    bottomBar:{
        backgroundColor: "#3f51b5",
        width: "100%",
        position: "absolute",
        bottom: "0px",
        color: "white"
    },
    spinnerPaper: {
        backgroundColor: "transparent",
        padding: theme.spacing(0, 0, 0),
        border: 'none'
    },
    newUserModalPaper: {
        padding: theme.spacing(0, 0, 0),
        width: "300px"
    },
    columns: {
        minWidth: 400
    },
    tabsContainer: {
        marginTop: 10
    },
    table: {
        minWidth: 200
    }
});

export class Profile extends React.Component{
    
    constructor(props){
        super(props);
        this.state = {
            countryIdx: "",
            stateIdx: "",
            cityIdx: "",
            tabValue: 0,
            states: [],
            cities: [],
            countries: countries.countries,
            user: userModel.model,
            validations: {
                "nameRequired": {
                    validation: () =>  { return this.state.user.name.trim() === "" },
                    errorMessage: "Required"
                },
                "usernameRequired": {
                    validation: () =>  { return this.state.user.username.trim() === "" },
                    errorMessage: "Required"
                }
            },
            modal: false,
            modalDeleteBlob: false, 
            modalEditBlob: false,
            files:[],
            showDocTab: false,
            deleteId: "",
            photoUrl: "",
            editfiles: add,
            isloading: false
        }
    }

    handleDeleteInfo = async() => {
        const { deleteId } = this.state;
        await blobsApi.DeleteBlob(deleteId)
        .then(() => {
            var idx = this.state.files.findIndex((e) => e.id === deleteId);
            if(idx > -1) {
                var temp = [...this.state.files];
                temp.splice(idx,1);
                this.setState({
                    files: temp
                })
            }
            this.setState({
                modalDeleteBlob: false,
                photoUrl: ""
            })
        })
        .catch((error) => {
            alert(error)
        })
    }

    bodyModalDelete = () => {
        const { handleCloseModal, handleDeleteInfo } = this;
        return(
            <Grid container >
                <Grid item xs={12} >
                    <h3>
                        ¿Estás seguro de borrar?
                    </h3>
                </Grid>
                <Grid item xs={12} >
                    <CustomButton
                    content={"Accept"}
                    color={"primary"}
                    handleClick={handleDeleteInfo}
                    />{" "}
                    <CustomButton
                    content={"Cancel"}
                    color={"secondary"}
                    handleClick={handleCloseModal}
                    />
                </Grid>
            </Grid>
            
            
        )
    }

    handleChange = (e) =>{
        const {id, value} = e.target;
        this.setState((prevState) => ({
            ...prevState, 
            user: {
                ...prevState.user,
                [id]: value
            }
        }));
        userModel.model = this.state.user
    } 

    handleChangeTab = (event, newValue) => {        
        this.setState({
            tabValue: newValue,
        })
    }

    componentDidMount(){
        this.setState({
            dumpTime: true
        }, () => {
            this.loadUserData();
        })
    }

    handleCountry = (countryIdx) => {
        var idx = this.state.countries.findIndex((e) => e.id === countryIdx)
        if (idx > -1){
            this.setState({
                countryIdx: idx,
                states: states.states[countryIdx]
            })
        }
    }

    handleState = (stateIdx) => {
        var idx = this.state.states.findIndex((e) => e.id === stateIdx)
        if (idx > -1){
            this.setState({
                stateIdx: idx,
                cities: cities.cities[stateIdx]
            })
        }
    }

    handleCity = (cityIdx) => {
        var idx = this.state.cities.findIndex((e) => e.id === cityIdx)
        if (idx > -1){
            this.setState({
                cityIdx: idx
            })
        }
    }    

    loadUserData = async() => {
        this.setState({
            isloading: true
        })
        /* var model = {
            query: "select * from Blobs"
        }
        var flag = false;
        await blobsApi.GetAll(model)
        .then((result) => {
            this.setState({
                files: result.models
            })
            flag = true;
        })
        .catch((error) => {
            alert(error)
        })
        if (flag) {
            for (let index = 0; index < this.state.files.length; index++) {
                this.showImage(index);
            }
        } */
        var id = this.props.userAuth.id;
        var email = this.props.userAuth.email;
        this.setState((prevState) => ({
            ...prevState, 
            user: {
                ...prevState.user,
                id: id,
                email: email
            }
        }));
        await usersApi.Get(id)
        .then((result) => {
            var tempCountry = result.country
            this.handleCountry(tempCountry)

            var tempState = result.state
            this.handleState(tempState)

            var tempCity = result.city
            this.handleCity(tempCity)

            this.setState({
                user: result,
                showDocTab: false,
                found: true
            })
            userModel.model = result;
        })
        .catch((error) => {
            this.setState({
                showDocTab: false
            })
            if (error !== constants.NotFound && error !== constants.NotAuthorized)  {
                alert(error)
            }
        })
        .finally(() => {
            this.setState({
                isloading: false,
            });
        })
    }

    showImage = async(index) => {
        var flagToken = false;
        var modelToken = await blobsApi.GetSASToken(this.state.files[index].blobName, permissionFile.Read, 1)
        .then((result) =>  {
            flagToken = true;
            return result;
        })
        .catch((error) => {
            alert(error)
        })
        if (flagToken) {
            this.state.files[index].url = modelToken.uri
            this.userPhoto()
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
        const { id, value } = element.target;
        if (id === "country") {
            this.checkCountry(value)
        }
        else if (id === "state") {
            this.checkCities(value)
        }
    }

    checkCountry = (value) => {
        this.setState({
            states: states.states[value],
            cities: []
        })
    }

    checkCities = (value) => {
        this.setState({
            cities: cities.cities[value]
        })
    }

    handleOpenModal = () => {
        this.setState({
            modal: true
        })
    }

    handleOpenModalDelete = (id) => {
        this.setState({
            modalDeleteBlob: true,
            deleteId: id
        })
    }

    handleOpenModalEdit = (id, files) => {
        this.setState({
            modalEditBlob: true,
            editfiles: files
        })
    }

    handleCloseModal = () => {
        this.setState({
            modal: false,
            modalDeleteBlob: false,
            modalEditBlob: false
        })
    }

    userPhoto = () => {
        const { files } = this.state;
        var idx = files.findIndex((e) => e.fileType === "Photo")
        if (idx > -1) {
            var temp = [...files]
            var url =  temp[idx].url
        }
        this.setState({
            photoUrl: url
        })
    }

    renderLocalization = () => {
        const { states, cities, cityIdx, stateIdx, countryIdx, validations, countries } = this.state;
        const { handleChangeLocation } = this;
        const { classes } = this.props;
        return(
            <Grid item sm={6} className={classes.columns} style={{paddingTop: 100}}>
                <div style={{padding: '20px'}}>
                <CustomSelect 
                    id={"country"} 
                    labelId={"Country-label"} 
                    label={"Country"}
                    idx={countryIdx}
                    idxState={"countryIdx"}
                    fieldToShow={"name"}
                    idItem={"country"}
                    items={countries}
                    handleChange={handleChangeLocation}
                    errorConditions={
                        <Fragment>
                            {validations["requiredCountry"].validation() && validations["requiredCountry"].errorMessage.concat("\n")} 
                        </Fragment>
                    } 
                />
                </div>
                <div style={{padding: '20px'}}>
                <CustomSelect 
                    id={"state"} 
                    labelId={"State-label"} 
                    label={"State"}
                    idx={stateIdx}
                    idxState={"stateIdx"}
                    fieldToShow={"name"}
                    idItem={"state"}
                    items={states}
                    handleChange={handleChangeLocation}
                    errorConditions={
                        <Fragment>
                            {validations["requiredState"].validation() && validations["requiredState"].errorMessage.concat("\n")} 
                        </Fragment>
                    } 
                />
                </div>
                <div style={{padding: '20px'}}>
                <CustomSelect 
                    id={"city"} 
                    labelId={"City-label"} 
                    label={"City"}
                    idx={cityIdx}
                    idxState={"cityIdx"}
                    fieldToShow={"name"}
                    idItem={"city"}
                    items={cities}
                    handleChange={handleChangeLocation}
                    errorConditions={
                        <Fragment>
                            {validations["requiredCity"].validation() && validations["requiredCity"].errorMessage.concat("\n")} 
                        </Fragment>
                    } 
                />
                </div>
            </Grid>   
        )
    }

    renderInfoGeneral = () => {
        const { id, email, name, username } = this.state.user;
        const { handleChange } = this;
        const { classes } = this.props;
        const { photoUrl, validations } = this.state;
        return(
            <Grid item sm={6} className={classes.columns} style={{paddingRight: "50px"}}>

               {/*  <CustomImage url={photoUrl} /> */}

                <CustomTextField 
                    id={"id"} 
                    value={id} 
                    label={"Id"}  
                    disabled
                />
                <CustomTextField 
                    id={"email"} 
                    value={email} 
                    label={"email"} 
                    disabled 
                />
                <CustomTextField 
                    id={"name"} 
                    value={name} 
                    label={"Name"}  
                    handleChange={handleChange}
                    errorConditions={
                        <Fragment>
                            {validations["nameRequired"].validation() && validations["nameRequired"].errorMessage.concat("\n")} 
                        </Fragment>
                    } 
                />
                <CustomTextField 
                    id={"username"} 
                    value={username} 
                    label={"Username"}  
                    handleChange={handleChange}
                    errorConditions={
                        <Fragment>
                            {validations["usernameRequired"].validation() && validations["usernameRequired"].errorMessage.concat("\n")} 
                        </Fragment>
                    } 
                />
            </Grid>
        )
    }

    renderTabDocuments = () => {
        const { handleOpenModal, renderTableList } = this;
        return(
            <Fragment>
                <div style={{textAlign: "right"}}>
                    <CustomIcon Icon={icons.icon.AddIcon} handleClick={handleOpenModal} fontSize={"large"} />
                </div>
                {renderTableList()}
            </Fragment>
        )
    }

    renderHeadCells = (element) => {
        const { classes } = this.props;
        return(
            <Grid className={classes.table} item xs={2} >
                {element.title}
            </Grid>
        )
    }

    renderBodyCells = (row) => {
        const { classes } = this.props;
        const { handleOpenModalDelete, handleOpenModalEdit } = this;
        return(
            <Fragment>
                <Grid className={classes.table}  item xs={2} >
                    <CustomImage url={row.url} />                                                   
                </Grid>
                <Grid className={classes.table} item xs={2} >{row.id}</Grid>
                <Grid className={classes.table} item xs={2} >{row.filename}</Grid>
                <Grid className={classes.table} item xs={2} >{row.fileExtension}</Grid>
                <Grid className={classes.table} item xs={2} >{row.fileType}</Grid>
                <Grid className={classes.table} item xs={2} >
                    <CustomIcon Icon={icons.icon.EditIcon}  handleClick={(e) => handleOpenModalEdit(row.id, row)}/>
                    <CustomIcon Icon={icons.icon.DeleteIcon} color={"secondary"} handleClick={(e) => handleOpenModalDelete(row.id)} />
                </Grid>
            </Fragment>
        )
    }

    renderTableList = () => {
        const { renderHeadCells, renderBodyCells } = this;
        const { files } = this.state;
        var headFile = [
            {
                title: "Image",
                size: 1
            },
            {
                title: "ID",
                size: 2
            },
            {
                title: "FIle Name",
                size: 3
            },
            {
                title: "File Extension",
                size: 4
            },
            {
                title: "Type",
                size: 5
            },
            {
                title: "Actions",
                size: 6
            }
        ]
        return(
            <CustomList headCell={renderHeadCells} bodyCells={renderBodyCells} files={files} headFile={headFile} />
        )
    }
    
    renderTabGeneral = () => {
        const {renderInfoGeneral, renderLocalization } = this;
        const { classes } = this.props;

        return(
            <div className={classes.tabsContainer}>
                <Grid container>
                    {renderInfoGeneral()}
                    {/* {renderLocalization()}   */}
                </Grid>
            </div>
        )
    }

    renderTabs = () => {
        const tabs = [
            {
                title: "Information",
                item: this.renderTabGeneral()
            },
            /* {
                title: "Documents",
                item: this.renderTabDocuments()
            } */
        ]
        return(
            <CustomSwiper tabs={tabs} />
        )
    }

    insertFile = (item) => {
    var temp = [...this.state.files];
    temp.push(item)
    if (item.fileType === "Photo") {
    this.setState({
        photoUrl: item.url
    })
    }
    this.setState({
        files: temp,
        modal: false
    })
    }

    updateFile = (item) => {
    var temp = [...this.state.files];
    var idx = temp.findIndex((e) => e.id === item.id);
    if (idx > -1) {
        temp[idx].id = item.id;
        temp[idx].filename = item.filename;
        temp[idx].fileExtension= item.fileExtension;
        temp[idx].fileType = item.fileType;
    }
    this.setState({
            files: temp,
            modalEditBlob: false,
            photoUrl: item.url
        })
        if (item.fileType !== "Photo") {
        this.setState({
            photoUrl: ""
        })
        }
    }

    createUser = async() =>{
        this.setState({
            isloading: true
        });
        await usersApi.Create(this.state.user)
        .then((result) => {
            alert("The user has been created")
        })
        .catch((error) => {
            alert(error)
        })
        .finally(() => {
            this.setState({
                isloading: false,
                found: true
            });
        });
    }

    updateUser = async() =>{
        this.setState({
            isloading: true
        });
        await usersApi.Update(this.state.user)
        .then((result) => {
            alert("The user has been updated")
        })
        .catch((error) => {
            alert(error)
        })
        .finally(() => {
            this.setState({
                isloading: false
            });
        });
        }

    render(){
        const { handleCloseModal, insertFile, bodyModalDelete, updateFile, createUser } = this;
        const { modal, modalDeleteBlob, modalEditBlob, editfiles, isloading, found } = this.state;
        const { classes } = this.props;
        return(
            <Fragment>
                 <Container>
                    {this.renderTabs()}
                    {isloading && 
                        <CustomSpinner open={true} paperClass={classes.spinnerPaper} />
                    }
                </Container>
                <CustomModal modal={modal} paperClass={classes.newUserModalPaper}
                    item={<CustomDragAndDrop insertFile={insertFile} editFiles={add} handleCloseModal={handleCloseModal} />}
                />
                <CustomModal modal={modalDeleteBlob} handleCloseModal={handleCloseModal} 
                    item={bodyModalDelete()}
                />
                <CustomModal modal={modalEditBlob} paperClass={classes.newUserModalPaper}
                    item={<CustomDragAndDrop editFiles={editfiles} updateFile={updateFile} handleCloseModal={handleCloseModal}/>}
                />
                <BottomBar classes={classes} isNotValid={this.validaData()} clickAction={found ? this.updateUser : createUser } />
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
        dispatch
    };
};

export default compose(
    withRouter,
    withStyles(useStyles, { withTheme: true }),
    connect(mapStateToProps, mapDispatchToProps)
    )(Profile);
