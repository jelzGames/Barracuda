import { withStyles } from "@material-ui/styles";
import React from "react";
import { compose } from "redux";
import CustomLink from "../common/customLink";

const useStyles = theme => ({
    root: {
        position: "absolute",
        width: "100%",
        bottom: "40%",
        left: "33%"
    },
    paper: {
      height: 140,
      width: 100,
    },
    control: {
      padding: theme.spacing(2),
    },
  });

export class Home extends React.Component{

    render(){
        const { classes } = this.props;
        return(
            <div className={classes.root}>
                <h2>
                    Welcome to Barracuda Examples           
                </h2>
                <div style={{paddingBottom: "20px"}}>
                    In this React example you will be able to know how our open source Barracuda Solution works<br/>
                    with the authentification protocols 0Auth 2.0 and openID.<br></br>
                    If you want to know more about us just click in the links below.
                </div>
                <CustomLink content={"www.chambapatodos.com"} href={"https://www.chambapatodos.com/"}/><br/>
                <CustomLink content={"Documentation"} href={"https://www.chambapatodos.com/?epkb_post_type_1=identity-provider"}/>
            </div>         
        )
    }
}

export default compose(
    withStyles(useStyles, { withTheme: true }),
    )(Home);
