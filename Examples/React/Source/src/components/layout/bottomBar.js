import React from 'react';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import * as usersApi from "../../api/usersApi";
import userModel from "../../datatest/userModel";
import { withStyles } from '@material-ui/styles';
import clsx from 'clsx';
import CustomIcon from "../common/customIcon";
import * as icons from "../libraries/icons";

const drawerWidth = 169;
const margin = 95;
const useStyles = theme => ({
    toolbar: {
    paddingRight: 24
    },
    appBar: {
    width: `calc(100% - ${margin}px )`,
    top: "auto",
    bottom: 0,
    zIndex: theme.zIndex.drawer + 1,
    transition: theme.transitions.create(['width', 'margin'], {
        easing: theme.transitions.easing.sharp,
        duration: theme.transitions.duration.leavingScreen
    }),
    left: `${margin}px`,
    alignItems: "flex-end"
    },
    appBarShift: {
    marginLeft: drawerWidth,
    width: `calc(100% - ${drawerWidth}px - ${margin}px)`,
    transition: theme.transitions.create(['width', 'margin'], {
        easing: theme.transitions.easing.sharp,
        duration: theme.transitions.duration.enteringScreen
    }),
    }
});

export class BottomBar extends React.Component{

    handleSaveInfo = () => {
        const { id, name, username, city, state, country } = userModel.model;
        
        var model = {
            id: id,
            name: name,
            username: username,
            city: city,
            state: state,
            country: country
    }

    if(name !== "" && username !== "" && city !== "" && state !== "" && country !== ""){
        usersApi.Create(model)
        .then((result) => {
            this.setState({
                showDocTab: true
            })
        })
        .catch((error) => {
            console.log(error)
        })
    }
}

    render(){
        const { handleSaveInfo } = this;
        const { classes, open } = this.props;
        return(
            <div className={classes.bottomBar}>    
                    <div >
                        <CustomIcon 
                            Icon={icons.icon.SaveIcon} 
                            fontSize={"large"}  
                            handleClick={handleSaveInfo}
                            color={"inherit"}
                            newClass={classes.saveIcon}
                        />
                    </div>
            </div>
        )
    }
}

export default withStyles(useStyles, { withTheme: true })(BottomBar);