import React from 'react';
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableContainer from '@material-ui/core/TableContainer';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';
import Paper from '@material-ui/core/Paper';
import { withStyles } from '@material-ui/styles';
import Button from '@material-ui/core/Button';
import AddIcon from '@material-ui/icons/Add';
import * as usersApi from "../../api/usersApi";
import CustomSpinner from '../common/customSpinner';
import { Fragment } from 'react';
import * as usersAuthApi from "../../api/usersAuthApi";
import { Grid } from '@material-ui/core';
import { CustomButton } from '../common/customButton';
import CustomModal from '../common/customModal';
import CustomChangePassword from '../common/customChangePassword';


const useStyles = theme => ({
    table: {
      minWidth: 300,
    },
    spinnerPaper: {
        backgroundColor: "transparent",
        padding: theme.spacing(0, 0, 0),
        border: 'none'
    }
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
        await usersApi.GetAll(model)
        .then((result) => {
            this.setState({
                data: result.models
            })
        })
        .catch((error) => {
            console.log(error)
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
            console.log(error)
        })
        if(flag){
            await usersAuthApi.DeleteUser(this.state.id)
            .then((result) => {
            })
            .catch((error) => {
                console.log(error)
            })
            .finally(() => {
                this.setState({
                    isloading: false,
                    openModalDelete: false
                });
            })
        }
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
            modelPassword: element
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

    render() {
        const { data, isloading, openModalDelete, openModalPassword } = this.state;
        const { classes } = this.props;
        const {handleOpen } = this;
        return (
            <Fragment>
                {isloading && 
                    <CustomSpinner open={true} paperClass={classes.spinnerPaper} />
                }
                <Button variant='contained' startIcon={<AddIcon/>} color='primary' onClick={(e) => handleOpen("new")}>
                Add New User
                </Button><br/><br/>
                <TableContainer component={Paper}>
                    <Table className={classes.table}>
                        <TableHead>
                            <TableRow>
                                <TableCell align="center">ID</TableCell>
                                <TableCell align="center">Name</TableCell>
                                <TableCell align="center">Username</TableCell>
                                <TableCell align="center">Actions</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>                       
                            {data.map((element, idx) => (
                                <TableRow key={"userlist" + idx}>
                                    <TableCell align="center">{element.id}</TableCell>
                                    <TableCell align="center">{element.name}</TableCell>
                                    <TableCell align="center">{element.username}</TableCell>
                                    <TableCell align="center">
                                        <Button color="primary" variant="contained" onClick={(e) => this.handleOpen(element.id)}>Edit</Button>{"  "}
                                        <Button color="secondary" variant="contained" onClick={(e) => this.handleModalOpen(element.id)}>Delete</Button>{" "}
                                        <Button variant="contained" onClick={(e) => this.handleModalPassword(element)}>Change Password</Button>
                                    </TableCell>
                                </TableRow> 
                            )) 
                            }
                        </TableBody>
                    </Table>
                </TableContainer>
                <CustomModal modal={openModalDelete}
                    item={this.renderModalDelete()}
                />
                <CustomModal modal={openModalPassword} handleCloseModal={this.handleModalClose} 
                        item={<CustomChangePassword modelPassword={this.state.modelPassword} />}
                    /> 
            </Fragment>
        )
    }
}

export default withStyles(useStyles, { withTheme: true })(Users);




