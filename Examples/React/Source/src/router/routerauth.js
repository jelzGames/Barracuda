import React from "react";
import { Route, withRouter } from "react-router-dom";
import { compose } from "redux";
import CustomLogin from "../components/common/customLogin";
import { connect } from "react-redux";
import CustomModal from "../components/common/customModal";
import * as constants from "../constants";

const RequireUserComponent = ({ component, ...rest }) => {
  var modal = true;
  const renderFn = (Component, props) => () => {
    var userAuth = localStorage.getItem(constants.UserAuth);
    if (userAuth !== "undefined") {
      userAuth = JSON.parse(userAuth);
    }
    console.log(userAuth)
    if (Component && userAuth && userAuth.id  && userAuth.validEmail && !userAuth.block) {
      return <div>
              <Component {...props} />
             </div>;
    }
    else {
      return <div>
                <CustomModal modal={modal} 
                  item={<CustomLogin />} 
                />
             </div>;
    }
  };
  
  return <Route {...rest} render={renderFn(component, rest)} />;
};

const mapStateToProps = (state) => {
  return {
      userAuth: state.userAuth
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
      dispatch
  };
};

export const RequireUser = compose(
  withRouter,
  connect(mapStateToProps, mapDispatchToProps)
  )(RequireUserComponent);