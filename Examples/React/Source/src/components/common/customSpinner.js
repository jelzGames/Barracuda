import React from "react";
import CircularProgress from "@material-ui/core/CircularProgress";
import Fade from "@material-ui/core/Fade";
import { withStyles } from "@material-ui/core/styles";
import CustomModal from "../common/customModal";

const useStyles = theme => ({
    modal: {       
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
    },
    modalPaper: {
        backgroundColor: "white",
        borderRadius: "5px",
    },
    progressCircle: {
        size: 40,
    }
});

export class Spinner extends React.PureComponent {
    render() {
        const { classes, open, paperClass, circleClass } = this.props;
        return (
            <CustomModal
                modal={open}
                paperClass={paperClass}
                item={
                    <Fade in={open}>
                        <div className={`${classes.modalPaper} ${paperClass}`}>
                            <CircularProgress  className={`${classes.progressCircle} ${circleClass}`}/>
                        </div>
                    </Fade>
                } 
            />
        );
    }
}

export default withStyles(useStyles, { withTheme: true })(Spinner);

