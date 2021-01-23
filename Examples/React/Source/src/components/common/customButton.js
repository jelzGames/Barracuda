import React from 'react';
import { Button } from '@material-ui/core';
import { withStyles } from '@material-ui/styles';
import { withTranslation } from "react-i18next";
import { compose } from "redux";

const useStyles = theme => ({
    root: {
        '& > *': {
          margin: theme.spacing(2,0),
        },
      },
});

export class CustomButton extends React.Component {
    constructor(props){
        super(props);
        this.state = {
        }
    }

    render() {
        const { disabled, color, startIcon, handleClick, content, t, autoFocus, fullWidth} = this.props;
        return(
            <Button 
                disabled={disabled}
                color={color} 
                startIcon={startIcon} 
                autoFocus={autoFocus}
                variant="contained" 
                size="large" 
                onClick={handleClick}
                fullWidth={fullWidth}
            >
                {t(content)}
            </Button>
        )
    }
}

export default compose(
    withTranslation("common"),
    withStyles(useStyles, { withTheme: true }),
)(CustomButton);
