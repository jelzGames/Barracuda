import React from "react";
import CircularProgress from "@material-ui/core/CircularProgress";
import {Modal} from "@material-ui/core";
import Backdrop from "@material-ui/core/Backdrop";
import Fade from "@material-ui/core/Fade";
import { withStyles } from "@material-ui/core/styles";

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
        const { classes } = this.props;
        return (
            <Modal
                aria-labelledby="transition-modal-title"
                aria-describedby="transition-modal-description"
                className={classes.modal}
                open={this.props.open}
                closeAfterTransition
                BackdropComponent={Backdrop}
                BackdropProps={{
                    timeout: 500,
                }}
            >
                <Fade in={this.props.open}>
                    <div className={classes.modalPaper}>
                        <CircularProgress  className={classes.progressCircle}/>
                    </div>
                </Fade>
            </Modal>
        );
    }
}

export default withStyles(useStyles, { withTheme: true })(Spinner);

