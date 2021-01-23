import React from "react";
import { withStyles } from "@material-ui/styles";
import IconButton from '@material-ui/core/IconButton';


const useStyles = (theme) => ({
    root: {
      '& > span': {
        margin: theme.spacing(2),
      },
    },
  });

export class CustomIcon extends React.Component{
    constructor(props){
        super(props);
        this.state = {
        }
    }

    render(){
      const { Icon, color, fontSize, handleClick, disabled, classes, newClass } = this.props;
      return(
        <div className={`${classes.root} ${newClass}`}>
          <IconButton onClick={handleClick} disabled={disabled} color={color === "inherit" ? color : "primary"} >
            <Icon
              color={color ? color : "primary"}
              fontSize={fontSize}
            />
          </IconButton> 
        </div>
            
      )
    }
}

export default 
withStyles(useStyles, { withTheme: true })
(CustomIcon);