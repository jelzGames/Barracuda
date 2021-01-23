import React from 'react';
import { Typography } from '@material-ui/core';
import { withStyles } from '@material-ui/styles';
import { withTranslation } from "react-i18next";
import { compose } from "redux";

const useStyles = theme => ({
    errorMessage: {
        color: "rgba(255, 1, 1, 0.54)"
    },
});

export class CustomTypography extends React.Component {
    constructor(props){
        super(props);
        this.state = {
        }
    }

    render() {
        const { classes, id, content, isError } = this.props;
        return(
            <Typography 
                id={id} 
                className={isError ? classes.errorMessage : {}}
                style={{whiteSpace: 'pre-line'}}
            >
                {content}
            </Typography>
        )
    }
}

export default compose(
    withTranslation("common"),
    withStyles(useStyles, { withTheme: true }),
)(CustomTypography);

