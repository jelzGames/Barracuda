import React from 'react';
import { TextField } from '@material-ui/core';
import { withStyles } from '@material-ui/styles';
import { withTranslation } from "react-i18next";
import { compose } from "redux";
import ErrorConditions from "./errorConditions";
import * as formatHelpers from "../../helpers/formaters";

const useStyles = theme => ({
    measure: {
        width: "100%",
        margin: theme.spacing(1,0)
    }
});

export class CustomTextField extends React.Component {
    constructor(props){
        super(props);
        this.state = {
            value: this.props.value,
            isNumber: this.props.isNumber,
            isFocus: false,
            valueNumberText: this.props.value.toString()
        }
    }

    componentDidMount() {
        if (this.state.isNumber === "number") {
            this.textNumber();
        }
    }

    componentDidUpdate(prevProps){
        if(prevProps.value !== this.props.value){
            this.setState({
                value: this.props.value
            })
        }
    }

    handleChange = (e) => {
        const { value } = e.target;
        this.setState({
            value: value
        }, () => {
            this.props.handleChange(e);
        })
    } 

    textNumber = () => {
        const { i18n, t } = this.props;
        this.setState({
            valueNumberText: formatHelpers.NumberFormat(this.state.value, this.props.maxDecimals, i18n, t)
        })
    }

    onBlur = () => {
        if (this.state.isNumber === "number") {
            this.setState({
                isFocus: false,
                value: this.state.value === "" ? 0 : this.state.value,
            }, () => {
                this.textNumber();
            })
        }
    }

    onFocus = () => {
        if (this.state.isNumber === "number") {
            this.setState({
                isFocus: true
            })
        }
    }

    renderText = () => {
        const { id, label, isNumber, t, autoFocus, classes, disabled, variant } = this.props;
        const { value } = this.state;
        const { handleChange, onBlur } = this;
        return (
            <TextField
                InputLabelProps={{ shrink: true }} 
                id={id} 
                label={t(label)}
                value={value}
                autoFocus={autoFocus}
                type={isNumber === "number" ? "number" : isNumber}
                onChange={handleChange}
                onBlur={onBlur}
                className={classes.measure}
                disabled={disabled}
                variant={variant}
            />            
        )
    }

    renderReadOnly = () => {
        const { id, label, t, classes } = this.props;
        const { valueNumberText } = this.state;
        const { onFocus } = this;
        return (
            <TextField 
                InputLabelProps={{ shrink: true }} 
                id={"readOnly-" + id} 
                label={t(label)}
                value={valueNumberText}
                readOnly
                type={""}
                onFocus={onFocus}
                className={classes.measure}
            />       
        )
    }

    render() {
        const { errorConditions } = this.props;
        const { isNumber, isFocus } = this.state;
        return(
            <div>
                {!isNumber || (isNumber !== "number" && isFocus) || isNumber !== "number" ? ( 
                   this.renderText()
                ) : (
                    this.renderReadOnly()
                )}
                {errorConditions &&
                    <ErrorConditions id="error-helper-text" errorConditions={errorConditions}/>
                }
            </div>
        )
    }
}

export default compose(
    withTranslation("common"),
    withStyles(useStyles, { withTheme: true }),
)(CustomTextField);

