import React from 'react';
import { withStyles } from "@material-ui/core/styles";
import Container from '@material-ui/core/Container';

const useStyles = theme => ({
    content: {
        height: '100vh',
        overflow: 'auto',
        maxWidth: "100%" 
      },
      container: {
        paddingTop: theme.spacing(4),
        paddingBottom: theme.spacing(4),
        paddingLeft: theme.spacing(0),
        paddingRight: theme.spacing(0)
      },
      paper: {
        padding: theme.spacing(2),
        display: 'flex',
        overflow: 'auto',
        flexDirection: 'column',
        width: "100%"
      },
      fixedHeight: {
        height: 240
      },
      appBarSpacer: theme.mixins.toolbar,
});

export class MainContainer extends React.Component {

    render() {
        const { classes } = this.props;
        return (
            <main className={classes.content}>
                <div className={classes.appBarSpacer} />
                  <Container  className={classes.container}>
                    {this.props.children}
                  </Container>
            </main>
        )
    }
}

export default withStyles(useStyles, { withTheme: true })(MainContainer);




