import React from 'react';
import { withStyles } from '@material-ui/styles';
import CustomIcon from "../common/customIcon";
import * as icons from "../libraries/icons";

const useStyles = theme => ({

});

export class BottomBar extends React.Component{

    render(){
        const { classes, isNotValid, clickAction} = this.props;
        return(
            <div className={classes.bottomBar}>    
                <CustomIcon
                    disabled={isNotValid}
                    Icon={icons.icon.SaveIcon} 
                    fontSize={"large"}  
                    handleClick={() => clickAction()}
                    color={"inherit"}
                    newClass={classes.saveIcon}
                />
            </div>
        )
    }
}

export default withStyles(useStyles, { withTheme: true })(BottomBar);