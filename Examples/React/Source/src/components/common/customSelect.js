import React from 'react';
import { InputLabel, Select, MenuItem } from '@material-ui/core';
import { withStyles } from '@material-ui/styles';
import { withTranslation } from "react-i18next";
import { compose } from "redux";
import ErrorConditions from "./errorConditions";

const useStyles = theme => ({
    measure: {
        width: "100%",
    },
});

export class CustomSelect extends React.Component {
    constructor(props){
        super(props);
        this.state = {
            idx: this.props.idx,
        }
    }

    componentDidUpdate(prevProps){
        if(prevProps.idx !== this.props.idx){
            this.setState({
                idx: this.props.idx
            })
        }
    }

    handleChange = (e) => {
        const { value } = e.target;
        this.setState({
            idx: value
        }, () => {
            this.props.handleChange(value, this.props.idxState,
            { 
                target: {
                    id: this.props.idItem,         
                    value: this.props.items[value].id         
                }
            });
        })
    } 

    render() {
        const { id, labelId, label, items, errorConditions, fieldToShow, t , autoFocus, classes} = this.props;
        const { idx } = this.state;
        const { handleChange } = this;
        return(
            <div>
                 <InputLabel id={labelId}>{t(label)}</InputLabel>
                 <Select
                    className={classes.measure}
                    labelId={labelId}
                    autoFocus={autoFocus}
                    id={id}
                    value={idx}
                    onChange={handleChange}
                >
                    {items.map((item, idx) => (
                        <MenuItem key={id + "-" + idx} id={id + "-" + idx} value={idx}>{item[fieldToShow]}</MenuItem>
                    ))}
                </Select>
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
)(CustomSelect);
