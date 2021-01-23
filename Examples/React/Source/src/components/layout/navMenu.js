import React, { Fragment } from 'react';
import Drawer from '@material-ui/core/Drawer';
import IconButton from '@material-ui/core/IconButton';
import { withStyles } from "@material-ui/core/styles";
import ChevronLeftIcon from '@material-ui/icons/ChevronLeft';
import Divider from '@material-ui/core/Divider';
import * as constants from "../../configFiles/constants";
import ListItems from "./listItems";
import clsx from 'clsx';

//import { mainListItems, secondaryListItems } from '../../configFiles/listItems';

const drawerWidth = constants.drawerWidth;



const useStyles = theme => ({
    drawerPaper: {
        position: 'relative',
        whiteSpace: 'nowrap',
        width: drawerWidth,
        transition: theme.transitions.create('width', {
          easing: theme.transitions.easing.sharp,
          duration: theme.transitions.duration.enteringScreen,
        }),
    },
    drawerPaperClose: {
        overflowX: 'hidden',
        transition: theme.transitions.create('width', {
          easing: theme.transitions.easing.sharp,
          duration: theme.transitions.duration.leavingScreen,
        }),
        width: theme.spacing(7),
        [theme.breakpoints.up('sm')]: {
          width: theme.spacing(9),
        },
    },
    toolbarIcon: {
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'flex-end',
        padding: '0 8px',
        ...theme.mixins.toolbar,
    },
});

export class NavMenu extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
          windowSize: window.innerWidth
        };
      }

      handleResize = () => {
        this.setState({
          windowSize: window.innerWidth
        })
    };

      componentDidMount() {
        window.addEventListener("resize", this.handleResize);
      }
    
      componentWillUnmount() {
        window.removeEventListener("resize", this.handleResize);
      }

    render() {
        const { classes, open, handleDrawerClose, handleDrawerOpen} = this.props;
        const {windowSize} = this.state;
        return (        
            <Fragment>
                {windowSize > 620 ? 
                <Drawer
                    variant="permanent"
                    classes={{
                        paper: clsx(classes.drawerPaper, !open && classes.drawerPaperClose),
                    }}
                    open={open}
                >
                    <div className={classes.toolbarIcon}>
                        <IconButton onClick={handleDrawerClose}>
                            <ChevronLeftIcon />
                        </IconButton>
                    </div>
                    <Divider />
                    <ListItems handleDrawerOpen={handleDrawerOpen}/>
                </Drawer>  :           
                <Drawer
                    variant="temporary"
                    classes={{
                        paper: clsx(classes.drawerPaper, !open && classes.drawerPaperClose),
                    }}
                    open={open}
                >
                    <div className={classes.toolbarIcon}>
                        <IconButton onClick={handleDrawerClose}>
                            <ChevronLeftIcon />
                        </IconButton>
                    </div>
                    <Divider />
                    <ListItems handleDrawerOpen={handleDrawerOpen}/>
                </Drawer>           
             } 

            </Fragment>
        )
    }
}

export default withStyles(useStyles, { withTheme: true })(NavMenu);




