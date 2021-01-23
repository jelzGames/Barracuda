import React from "react";
import { Divider, Grid } from "@material-ui/core";
import { withStyles } from "@material-ui/styles";
import { Fragment } from "react";

const useStyles = theme => ({
    paper: {
        padding: theme.spacing(2),
        textAlign: 'center',
        backgroundColor: theme.palette.background.paper
      },
      divider: {
        margin: theme.spacing(2, 0)
      },
      headers: {
        fontWeight: "bold"
      },
      alingCells: {
        justify: "center",
        alignItems: "center"
      }
  });

export class CustomList extends React.Component{
    render(){
        const { classes, headCell, files, bodyCells, direction, listClass, headerClass, dividerClass, bodyClass, headFile } = this.props;
        return(
            <Grid className={`${classes.paper} ${listClass}`} >
                <Grid container className={`${classes.headers} ${classes.alingCells} ${headerClass}`}  >
                    {headFile.map((element, idx) => (
                        <Fragment key={"Head-" + idx} >
                            {headCell(element)}
                        </Fragment>
                    ))
                    }
                </Grid>
                <Divider className={`${classes.divider} ${dividerClass}`} />
                <Grid container className={`${classes.alingCells} ${bodyClass}`} >
                    {files.map((element, idx) => (
                        <Grid container key={"files-" + idx } direction={ direction ? direction : "row"} >
                            {bodyCells(element)}
                        </Grid>    
                    )) 
                    }
                </Grid>
            </Grid>
        )
    }
} 

export default 
withStyles(useStyles, { withTheme: true })
(CustomList);