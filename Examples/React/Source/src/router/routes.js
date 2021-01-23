import React, { lazy, Suspense } from "react";
import { Switch } from "react-router-dom";
import{ RequireUser } from "./routerauth";
import CustomSpinner from "../components/common/customSpinner";

const Users = lazy(() => import("../components/users/users"));
const Roles = lazy(() => import("../components/roles/roles"));
const Workqueues = lazy(() => import("../components/workqueues/workqueues"));
const AddNewUser= lazy(() => import("../components/users/addedituser"));
const AddEditRoles= lazy(() => import("../components/roles/addEditroles"));
const Profile= lazy(() => import("../components/profiles/profile"));

export const routes = (
    <Switch>
        <Suspense fallback={<CustomSpinner open={true} />}>
            <RequireUser  exact={true} path="/" component={Profile}/> 
            <RequireUser exact={true} path="/users" component={Users} />
            <RequireUser exact={true} path="/users/:id" component={AddNewUser} />
            <RequireUser  exact={true} path="/roles" component={Roles} />
            <RequireUser  exact={true} path="/roles/:id" component={AddEditRoles}/>
            <RequireUser  exact={true} path="/workqueues" component={Workqueues} />          
        </Suspense>
    </Switch>
);


