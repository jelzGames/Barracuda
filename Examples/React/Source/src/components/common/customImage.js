import { withStyles } from "@material-ui/styles";
import React from "react";

const useStyles = theme => ({
      img: {
        display: 'block',
        width: 'auto',
        height: '100%'
      },
      previewInner: {
        display: 'flex',
        minWidth: 0,
        overflow: 'hidden'
      },
      preview: {
        display: 'inline-flex',
        borderRadius: 2,
        border: '1px solid #eaeaea',
        marginBottom: 8,
        marginRight: 8,
        width: 100,
        height: 100,
        padding: 4,
        boxSizing: 'border-box'
      }
  });

export class CustomImage extends React.Component{

    render(){
        const { classes, url, topPreview, bottomPreview, image } = this.props
        return(
            <div className={`${classes.preview} ${topPreview}`}>
                <div className={`${classes.previewInner} ${bottomPreview}`}>
                    <img 
                        src={url} 
                        alt=""
                        className={`${classes.img} ${image}`}
                    />
                </div>
            </div>   
        )
    }
}

export default 
withStyles(useStyles, { withTheme: true })
(CustomImage);