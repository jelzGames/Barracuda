import React from 'react';
import { Button } from '@material-ui/core';
import AddIcon from '@material-ui/icons/Add';
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableContainer from '@material-ui/core/TableContainer';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';
import Paper from '@material-ui/core/Paper';
import rolesData from "../../datatest/rolesData"
import EditIcon from '@material-ui/icons/Edit';
import DeleteIcon from '@material-ui/icons/Delete';
import { withStyles } from '@material-ui/styles';

const useStyles = theme => ({
    table: {
      minWidth: 300,
    },
  });

export class Roles extends React.Component {

    constructor(props){
        super(props);
        this.state = {
            Data: rolesData.Data
        }
    }

    handleOpen = (id) => {
        this.props.history.push(`/roles/${id}`)
    };

    handleDelete = (element) => {
        var idx = rolesData.Data.findIndex((e) => e.id === element.id);
        if (idx > -1) {
            rolesData.Data.splice(idx, 1);
            this.setState({
                Data: rolesData.Data
            })
        }
    }

    render() {
        const { Data } = this.state;
        const {  classes } = this.props;
        const {  handleOpen } = this;

        return (
            <div>
                <Button startIcon={<AddIcon/>} variant="contained" color="primary" onClick={(e) => handleOpen("new")}>
                    Add Roles
                </Button><br/><br/>
                <div>
                <TableContainer component={Paper}>
                    <Table className={classes.table}>
                        <TableHead>
                            <TableRow>
                                <TableCell align="center">ID</TableCell>
                                <TableCell align="center">Role</TableCell>
                                <TableCell align="center">Actions</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>                       
                            {Data.map((element, idx) => (
                                <TableRow key={"roleslist" + idx}>
                                    <TableCell align="center">{element.id}</TableCell>
                                    <TableCell align="center">{element.role}</TableCell>
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

export default withStyles(useStyles, { withTheme: true })(Roles);