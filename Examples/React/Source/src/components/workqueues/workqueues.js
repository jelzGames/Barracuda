import React from 'react';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemText from '@material-ui/core/ListItemText';
import * as usersApi from "../../api/usersApi";
import { compose } from "redux";
import * as constants from "../../constants";

export class Workqueues extends React.Component {

    componentDidMount(){
        this.checkUser();
    }

    checkUser = async() => {
        var id = this.props.user.sub;
        await usersApi.Get(id)
        .then((result) => {
            
        })
        .catch((error) => {
            if (error === constants.NotFound) {
                this.props.history.push(`/users/new`)
            }
        })
    }


    render() {
        return (
            <div>
                <List component="ul">
                    <ListItem>
                        <ListItemText primary="Raparación de Caldera"/>
                    </ListItem>
                    <ListItem>
                        <ListItemText primary="Construir un comedor"/>
                    </ListItem>
                    <ListItem>
                        <ListItemText primary="Arreglar baño"/>
                    </ListItem>
                    <ListItem>
                        <ListItemText primary="Guardaespaldas de tiempo completo"/>
                    </ListItem>
                </List>
            </div>
        )
    }
}

export default compose(
)(Workqueues);