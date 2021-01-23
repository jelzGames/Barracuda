import React from 'react';
import { withStyles } from '@material-ui/styles';
import { withTranslation } from "react-i18next";
import { compose } from "redux";
import CustomTypography from "./customTypography";

const useStyles = theme => ({
});


export class ErrorConditions extends React.Component {
    constructor(props){
        super(props);
        this.state = {
        }
    }

    render() {
        const { id, errorConditions } = this.props;
        return(
            <CustomTypography 
                id={id}
                isError={true}
                content={errorConditions}
            />
         )
    }
}

export default compose(
    withTranslation("common"),
    withStyles(useStyles, { withTheme: true }),
)(ErrorConditions);

