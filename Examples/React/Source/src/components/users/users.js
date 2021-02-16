import React from 'react';
import { withStyles } from '@material-ui/styles';
import * as usersApi from "../../api/usersApi";
import CustomSpinner from '../common/customSpinner';
import { Fragment } from 'react';
import * as usersAuthApi from "../../api/usersAuthApi";
import { Grid } from '@material-ui/core';
import CustomButton from '../common/customButton';
import CustomModal from '../common/customModal';
import CustomChangePassword from '../common/customChangePassword';
import {ValidScopes} from "../../helpers/scopesHelper";
import CustomList from "../common/customList";
import CustomIcon from '../common/customIcon';
import * as icons from "../libraries/icons";


const useStyles = theme => ({
    table: {
      minWidth: 300,
    },
    spinnerPaper: {
        backgroundColor: "transparent",
        padding: theme.spacing(0, 0, 0),
        border: 'none'
    },
  });

export class Users extends React.Component {
    constructor(props){
        super(props);
        this.state = {
            data: [],
            isloading: false,
            openModalDelete: false,
            openModalPassword: false
        }
    }

    componentDidMount(){
        this.loadData();
    }

    loadData = async() => {
        this.setState({
            isloading: true
        });
        var model = {
            query: "select * from Users"
        }
        var batch = [];
        await usersApi.GetAll(model)
        .then((result) => {
            this.setState({
                data: result.models
            }, () => {                
                for(var x = 0; x < result.models.length; x++){
                    batch.push(result.models[x].id);
                }
            })

        })
        .catch((error) => {
            alert(error)
        })
        var modelBatch = {
            batch: batch
        };
        await usersAuthApi.GetBatchAdditional(modelBatch)
        .then((result) => {
            var data = [...this.state.data];
            for(var x = 0; x < result.length; x++){
                var idx = data.findIndex((e) => e.id === result[x].id)
                if(idx > -1 ){
                    data[idx].block = result[x].block;
                }
            }
            this.setState({
                data: data
            })
        })
        .catch((error) => {
            alert(error)
        })
        .finally(() => {
            this.setState({
                isloading: false
            });
        })
    }

    handleOpen = (id) => {
        this.props.history.push(`/users/${id}`)
    }

    handleDelete = async() => {
        this.setState({
            isloading: true
        });
        var flag = false;
        await usersApi.Delete(this.state.id)
        .then(() => {
            flag = true;
            var idx = this.state.data.findIndex((e) => e.id === this.state.id);
            if(idx > -1) {
                var temp = [...this.state.data];
                temp.splice(idx,1);
                this.setState({
                    data: temp
                })
            }
        })
        .catch((error) => {
            alert(error)
        })
        if(flag){
            await usersAuthApi.DeleteUser(this.state.id)
            .then((result) => {
            })
            .catch((error) => {
                alert(error)
            })
            .finally(() => {
            })
        }
        this.setState({
            isloading: false,
            openModalDelete: false
        });
    }

    handleModalDelete = (id) => {
        this.setState({
            openModalDelete: true,
            id: id
        })
    }

    handleModalPassword = (element) => {
        this.setState({
            openModalPassword: true,
            usermodel: element
        })
    }

    handleModalClose = () => {
        this.setState({
            openModalDelete: false,
            openModalPassword: false
        })
    }

    renderModalDelete = () => {
        const { handleDelete, handleModalClose } = this;
        return(
            <Grid container >
                <Grid item xs={12} >
                    <h3>
                        Do you want to delete the user?
                    </h3>
                </Grid>
                <Grid item xs={12} >
                    <CustomButton
                    content={"Accept"}
                    color={"primary"}
                    handleClick={handleDelete}
                    />
                    <CustomButton
                    content={"Cancel"}
                    color={"secondary"}
                    handleClick={handleModalClose}
                    />
                </Grid>
            </Grid>    
        )
    }

    renderHeadCells = (element) => {
        const { classes } = this.props;
        return(
            <Grid className={classes.table} item xs={3} >
                {element.title}
            </Grid>
        )
    }

    renderTableList = () => {
        const { renderHeadCells, renderBodyCells } = this;
        const { data } = this.state;
        console.log(data)
        var headFile = [
            {
                title: "ID",
                size: 1
            },
            {
                title: "Name",
                size: 2
            },
            {
                title: "Username",
                size: 3
            },
            {
                title: "Actions",
                size: 4
            }
        ]
        return(
            <CustomList headCell={renderHeadCells} bodyCells={renderBodyCells} files={data} headFile={headFile} />
        )
    }

    renderBodyCells = (row) => {
        const { classes } = this.props;
        const { handleOpen, handleModalDelete, handleModalPassword, handleBlockUser } = this;
        return(
            <Fragment>
                <Grid className={classes.table} item xs={3} >{row.id}</Grid>
                <Grid className={classes.table} item xs={3} >{row.name}</Grid>
                <Grid className={classes.table} item xs={3} >{row.username}</Grid>
                <Grid className={classes.table} item xs={3} >
                    <CustomIcon Icon={icons.icon.EditIcon} disabled={!ValidScopes("users.edit")} handleClick={(e) => handleOpen(row.id)}/>
                    <CustomIcon Icon={icons.icon.DeleteIcon} disabled={!ValidScopes("users.delete")} color={"secondary"} handleClick={(e) => handleModalDelete(row.id)} />
                    <CustomIcon Icon={icons.icon.VpnKeyIcon} disabled={!ValidScopes("users.changePassword")} handleClick={(e) => handleModalPassword(row)} />
                    <CustomIcon Icon={row.block ? icons.icon.LockIcon : icons.icon.LockOpenIcon} disabled={true} />
                </Grid>
            </Fragment>
        )
    }

    render() {
        const { isloading, openModalDelete, openModalPassword } = this.state;
        const { classes } = this.props;
        const {handleOpen } = this;
        return (
            <Fragment>
                {isloading && 
                    <CustomSpinner open={true} paperClass={classes.spinnerPaper} />
                }
                <div style={{padding: "20px"}}>
                   <CustomButton
                        content={"Add New User"}
                        color={"primary"}
                        handleClick={(e) => handleOpen("new")}
                    /> 
                </div>                
                {this.renderTableList()}
                <CustomModal modal={openModalDelete}
                    item={this.renderModalDelete()}
                />
                <CustomModal modal={openModalPassword} handleCloseModal={this.handleModalClose} 
                        item={<CustomChangePassword userModel={this.state.usermodel} />}
                    />        
            </Fragment>
        )
    }
}

export default withStyles(useStyles, { withTheme: true })(Users);




