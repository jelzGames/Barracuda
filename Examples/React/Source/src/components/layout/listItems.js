import React, { Fragment } from 'react';
import * as itemsFile from "../../configFiles/itemsFile";
import * as arrays from "../../helpers/arraysOperations";
import Collapse from "@material-ui/core/Collapse";
import ListItem from '@material-ui/core/ListItem';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import ListItemText from '@material-ui/core/ListItemText';
import List from '@material-ui/core/List';
import ExpandLess from '@material-ui/icons/ExpandLess';
import ExpandMore from '@material-ui/icons/ExpandMore';
import { withStyles } from "@material-ui/core/styles";
import Link from "@material-ui/core/Link"

const useStyles = theme => ({
    nested: {
      paddingLeft: theme.spacing(9),
    },
});

export class ListItems extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            items: [],
            open: []
        }
    }

    componentDidMount() {
        this.reorder();
    }

    reorder = () => {
        var temp = itemsFile.items.sort(arrays.fieldSorter(["module", "name"]));
        const grouped = arrays.groupBy(temp, item => item.module);
        this.setState({
            items: grouped
        });
    }

    renderItems = (item, key) => {
        const { classes } = this.props;
        return (
            <Collapse in={this.state[key]} timeout="auto" unmountOnExit>
                <List component="div" disablePadding>
                    {item.map((item, idx) => 
                        <ListItem key={"item"+idx} button className={classes.nested}>
                            <Link href={item.path}><ListItemText primary={item.name} /></Link>                      
                        </ListItem>
                    )}
                </List>
            </Collapse>
        );
    }

    handleClick = (key) => {
        this.setState({
            [key]: !this.state[key]
        })
    };

    renderModules = () => {
        const items = [];
        const {handleDrawerOpen} = this.props;
        this.state.items.forEach((value, key) => items.push(
            <Fragment key={key}>
                <ListItem button onClick={() => this.handleClick(key)} >
                    <ListItemIcon onClick={handleDrawerOpen}>
                        {itemsFile.icons.find((e) => e.id === key).icon}
                    </ListItemIcon>
                    <ListItemText primary={key} />
                    {this.state[key] ? <ExpandLess /> : <ExpandMore />}
                </ListItem>
                {this.renderItems(value, key)}
            </Fragment>
        ));
        
        return items;
    }

    render() {
        return (
            <Fragment>
                <List>
                    {this.renderModules()}
                </List>
            </Fragment>
        )
    }
}

export default withStyles(useStyles, { withTheme: true })(ListItems);
