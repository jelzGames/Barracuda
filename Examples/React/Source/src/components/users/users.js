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
import EditIcon from '@material-ui/icons/Edit';
import DeleteIcon from '@material-ui/icons/Delete';
import AddIcon from '@material-ui/icons/Add';
import * as usersApi from "../../api/usersApi";


const useStyles = theme => ({
    table: {
      minWidth: 300,
    },
  });

export class Users extends React.Component {
    constructor(props){
        super(props);
        this.state = {
            data: []
        }
    }

    componentDidMount(){
        this.loadData();
    }

    loadData = async() => {
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
    }

    handleOpen = (id) => {
        this.props.history.push(`/users/${id}`)
    };

    handleDelete = (element) => {
        usersApi.Delete(element.id)
        .then(() => {
            var idx = this.state.data.findIndex((e) => e.id === element.id);
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
    }


    render() {
        const { data } = this.state;
        const { classes } = this.props;
        const {handleOpen } = this;
        return (
            <div>       
                <Button variant='contained' startIcon={<AddIcon/>} color='primary' onClick={(e) => handleOpen("new")}>
                Add New User
                </Button><br/><br/>
                <div>
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
                                        <Button startIcon={<EditIcon/>} color="primary" variant="contained" onClick={(e) => this.handleOpen(element.id)}>Edit</Button>{"  "}
                                        <Button startIcon={<DeleteIcon/>} color="secondary" variant="contained" onClick={(e) => this.handleDelete(element)}>Delete</Button>
                                    </TableCell>
                                </TableRow> 
                            )) 
                            }
                        </TableBody>
                    </Table>
                </TableContainer>
                </div>
            </div>
        )
    }
}

export default withStyles(useStyles, { withTheme: true })(Users);




