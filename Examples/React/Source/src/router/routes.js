import React, { lazy, Suspense } from "react";
import { Route, Switch } from "react-router-dom";
import{ RequireUser } from "./routerauth";
import CustomSpinner from "../components/common/customSpinner";

const Users = lazy(() => import("../components/users/users"));
const Roles = lazy(() => import("../components/roles/roles"));
const Workqueues = lazy(() => import("../components/workqueues/workqueues"));
const AddEditUser= lazy(() => import("../components/users/addedituser"));
const AddEditRoles= lazy(() => import("../components/roles/addEditroles"));
const Profile= lazy(() => import("../components/profiles/profile"));
const VerifyTokens = lazy(() => import("../components/verifyTokens"));
const Home = lazy(() => import("../components/home/home"));

export const routes = (
    <Switch>
        <Suspense fallback={<CustomSpinner open={true} />}>
            <RequireUser  exact={true} path="/" component={Home}/>
            <RequireUser  exact={true} path="/profile" component={Profile}/> 
            <RequireUser exact={true} path="/users" component={Users} />
            <RequireUser exact={true} path="/users/:id" component={AddEditUser} />
            <RequireUser  exact={true} path="/roles" component={Roles} />
            <RequireUser  exact={true} path="/roles/:id" component={AddEditRoles}/>
            <RequireUser  exact={true} path="/workqueues" component={Workqueues} />
            <Route  exact={true} path="/verifyTokens" component={VerifyTokens} />          
        </Suspense>
    </Switch>
);


