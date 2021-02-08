import React from "react";
import useBrowserContextCommunication from 'react-window-communication-hook';


const PostMessageHoc = (Component) => {  
    const WrappedComponent = (props) => {
        const [communicationState, postMessage] = useBrowserContextCommunication("channel");
       
        console.log(communicationState)
        return <Component 
        communicationState = {communicationState}
        postMessage = {postMessage}
        />;
    };
    
    return WrappedComponent;
};

export default PostMessageHoc;