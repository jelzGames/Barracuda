import React, { Fragment } from 'react';
import { withStyles } from '@material-ui/styles';
import { withTranslation } from "react-i18next";
import { compose } from "redux";

const useStyles = theme => ({
});

export class CustomHeader extends React.Component {
    constructor(props){
        super(props);
        this.state = {
        }
    }

    render() {
        const { content, t, size} = this.props;
        return(
            <Fragment>
                {size === 1 && 
                    <h1>{t(content)}</h1>
                }
                {size === 2 && 
                    <h2>{t(content)}</h2>
                }
            </Fragment>
        )
    }
}

export default compose(
    withTranslation("common"),
    withStyles(useStyles, { withTheme: true }),
)(CustomHeader);
