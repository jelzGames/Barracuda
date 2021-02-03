import DashboardIcon from '@material-ui/icons/Dashboard';
import ShoppingCartIcon from '@material-ui/icons/ShoppingCart';

export const items = [
    {
        name: "workqueues",
        path: "/workqueues",
        module: "workqueue",
    },
    {
        name: "users",
        path: "/users",
        module: "admin",
    }/* ,
    {
        name: "roles",
        path: "/roles",
        module: "admin"
    } */
]

export const icons = [
    {
        id: "admin",
        icon: <DashboardIcon/>
    },
    {
        id: "workqueue",
        icon: <ShoppingCartIcon/>
    }
]