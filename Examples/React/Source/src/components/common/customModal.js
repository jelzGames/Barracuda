import React from 'react';
import { withStyles } from '@material-ui/styles';
import { withTranslation } from "react-i18next";
import { compose } from "redux";
import Modal from "@material-ui/core/Modal";
import Backdrop from "@material-ui/core/Backdrop";

const useStyles = theme => ({
    modal: {
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        textAlign: "center"
    },
    paper: {
        backgroundColor: theme.palette.background.paper,
        padding: theme.spacing(2, 4, 3),
        outline: 'none'
    }
});

export class CustomModal extends React.Component {
    constructor(props){
        super(props);
        this.state = {
        }
    }

    render() {
        const { modal, item, classes, handleCloseModal, paperClass, modalClass, hideBackdrop } = this.props;
        return(
            <div>
                {modal && 
                    <Modal
                        aria-labelledby="transition-modal-title"
                        aria-describedby="transition-modal-description"
                        className={`${classes.modal} ${modalClass}`}
                        open={modal}
                        BackdropComponent={Backdrop}
                        onClose={handleCloseModal}
                        hideBackdrop={hideBackdrop}
                        BackdropProps={{
                            timeout: 500,
                        }}
                        closeAfterTransition
                    >       
                        <div className={`${classes.paper} ${paperClass}`}>
                            {item}
                        </div>
                    </Modal>
                }
            </div>
        )
    }
}

export default compose(
    withTranslation("common"),
    withStyles(useStyles, { withTheme: true }),
)(CustomModal);
