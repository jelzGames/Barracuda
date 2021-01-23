import { Link } from "@material-ui/core";
import React from "react";
import { withTranslation } from "react-i18next";


export class CustomLink extends React.Component{

    render(){
        const { handleClick, href, component, to, content, t } = this.props;
        return(
            <div>
                <Link href={href} component={component} to={to} onClick={handleClick}>
                    {t(content)}
                </Link>
            </div>
        )
    }
}

export default 
withTranslation("common")
(CustomLink);