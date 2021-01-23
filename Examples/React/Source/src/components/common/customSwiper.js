import React from "react"
import SwipeableViews from "./swiper";
import Tabs from '@material-ui/core/Tabs';
import Tab from '@material-ui/core/Tab';
import { withStyles } from "@material-ui/styles";
import Paper from '@material-ui/core/Paper';

const useStyles = theme => ({
    root: {
        flexGrow: 1,
        marginLeft: theme.spacing(0)
      },
});

export class CustomSwiper extends React.Component{
    constructor(props){
        super(props);
        this.state = {
            value: 0
        }
    }

    handleChange = (event, newValue) => {
        this.setState({
            value: newValue
        })
    }

    handleChangeIndex = (index) => {
        const { handleChange} = this.props;
    
        this.setState({
            value: index
        })
    
        if (handleChange) {
            handleChange(index);
        }
    };

    render(){
        const { handleChangeIndex, handleChange } = this;
        const { tabs, classes } = this.props;
        const { value } = this.state;
        return(
            <div>
                <Paper className={classes.root}>
                    <Tabs
                        indicatorColor="primary"
                        textColor="secondary"
                        variant="scrollable"
                        scrollButtons="on"
                        value={value}
                        onChange={handleChange}
                        className="scrollable-tabs-tabs"
                    >
                        {tabs.map((tab, idx) => {
                            return (<Tab label={tab.title} key={idx} />);
                        })}
                    </Tabs>
                </Paper>
                <SwipeableViews
                    onChangeIndex={handleChangeIndex} 
                    index={value}
                >
                    {tabs.map((tab, idx) => {
                        return(
                            <div key={"tab" + idx}>{tab.item}</div>
                        );
                    })}
                </SwipeableViews>
            </div>
        )
    }
}

export default withStyles(useStyles, { withTheme: true })(CustomSwiper);