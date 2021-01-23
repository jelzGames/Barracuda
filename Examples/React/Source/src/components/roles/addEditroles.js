import { Button } from "@material-ui/core";
import React from "react"; 
import ArrowBackIcon from '@material-ui/icons/ArrowBack';
import OutlinedInput from '@material-ui/core/OutlinedInput';
import SaveIcon from '@material-ui/icons/Save';
import CloseIcon from '@material-ui/icons/Close';
import { withStyles } from '@material-ui/styles';
import rolesData from "../../datatest/rolesData"

const useStyles = theme => ({
    root: {
      '& > *': {
        margin: theme.spacing(1),
      },
    },
  });

export class Addeditroles extends React.Component{

    constructor(props){
        super(props);
        this.state = {
            id: this.props.match.params.id,
            roleid: this.props.match.params.id === 'new' ? "" : this.props.match.params.id,
            role: ""
        }
    }

    componentDidMount(){
        if (this.state.id !== "new") {
            var item = rolesData.Data.find((e) => e.id === this.state.id)
            if (item) {
                this.setState({
                    roleid: item.id,
                    role: item.role
                })
            }
        }
    }

    handleChange = (e) => {
        const { id, value } = e.target;
        this.setState({
            [id]: value
        })
    }

    handleSave = () => {
        if (this.state.id === "new") {
            rolesData.Data.push({
                id: this.state.roleid,
                role: this.state.role
            })
        }
        else {
            var idx = rolesData.Data.findIndex((e) => e.id === this.state.id)
            if (idx > -1) {
                rolesData.Data[idx].id = this.state.roleid;
                rolesData.Data[idx].role = this.state.role;
            }
        }
    }

    render(){
        const { handleChange, handleSave} = this;
        const { classes } = this.props;
        const { roleid, role } = this.state;

        return(
            <div>
                <Button  color="primary" onClick={() => this.props.history.goBack()}>
                <ArrowBackIcon fontSize="large"/>
                </Button>
                <form className={classes.root} > 
                        <h2>ID:</h2>
                        <OutlinedInput id={"roleid"} value={roleid} type="text" onChange={handleChange} />
                        <h2>Role:</h2>
                        <OutlinedInput  id={"role"} value={role} type="text" onChange={handleChange}/><br/>
                        <Button color="primary" startIcon={<SaveIcon/>} variant="contained" size="large" onClick={handleSave}>
                            Save
                        </Button>
                        <Button color="secondary" startIcon={<CloseIcon/>} variant="contained" size="large" onClick={() => this.props.history.goBack()}>
                            Cancel
                        </Button>
                </form>
            </div>
        )
    }
}

export default withStyles(useStyles, { withTheme: true })(Addeditroles);