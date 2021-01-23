import { withStyles } from "@material-ui/styles";
import React from "react";
import Dropzone from 'react-dropzone';
import LinearProgress from '@material-ui/core/LinearProgress';
import { withRouter } from "react-router-dom";
import { connect } from "react-redux";
import { compose } from "redux";
import * as blobsApi from "../../api/blobsApi";
import { permissionFile, add } from "../../constants";
import CustomSelect from "../common/customSelect";
import * as constants from "../../constants";
import { v4 as uuidv4 } from 'uuid';
import * as icons from "../libraries/icons";
import CustomIcon from "../common/customIcon";
import CustomImage from "../common/customImage";
import CustomButton from "../common/customButton";
import { Fragment } from "react";

const useStyles = theme => ({
    dragAndDrop: {
        display: "flex",
        alignItems: "center",
        justifyContent: "center"
    },
    typography: {
        fontFamily: 'Arial',
        fontSize: 16,
        fontStyle: "bold"
      },
    preview: {
        display: 'inline-flex',
        borderRadius: 2,
        border: '1px solid #eaeaea',
        marginBottom: 8,
        marginRight: 8,
        width: 200,
        height: 200,
        padding: 4,
        boxSizing: 'border-box'
      },
      previewInner: {
        display: 'flex',
        minWidth: 0,
        overflow: 'hidden'
      },
      img: {
        display: 'block',
        width: 'auto',
        height: '100%'
      },
      closeIcon: {
        textAlign: "right"
      }
});

export class CustomDragAndDrop extends React.Component{
    constructor(props){
        super(props);
        this.state = {
            filesTemp:[],
            percent: 0,
            image:[],
            file: null,
            filesTypes: constants.filesTypes,
            fileIdx: "",
            fileProgress: true,
            editFiles: this.props.editFiles
        };
    }

    xmlHTTP = new XMLHttpRequest();
    reader = new FileReader();

    handleAbortFile = () => {
        this.xmlHTTP.abort();
        this.reader.abort();
    }

    handleCloseModal = () => {
        const { handleCloseModal } = this.props;
        if (this.state.fileProgress) {
            handleCloseModal();
        }
    }

    componentDidMount(){
        this.xmlHTTP.upload.addEventListener("progress", this.progress,);
        this.xmlHTTP.upload.addEventListener("load", this.transferComplete);
        this.xmlHTTP.addEventListener("error", this.error);
        this.xmlHTTP.upload.addEventListener("abort", this.abort);

        this.reader.addEventListener('loadstart', this.loadStartReader);
        this.reader.addEventListener('load', this.loadCompleteReader);
        this.reader.addEventListener('loadend', this.loadEnd);
        this.reader.addEventListener('progress', this.progressReader);
        this.reader.addEventListener('error', this.errorReader);
        this.reader.addEventListener('abort', this.abortReader);

        this.getEdit()
    }
    
    getEdit = () => {
        if (this.state.editFiles !== add ) {
            var idx = constants.filesTypes.findIndex((e) => e.file === this.state.editFiles.fileType)
            this.setState({
                fileIdx: idx,
                file: this.state.editFiles
            })
        }
    }

    componentWillUnmount(){
        this.xmlHTTP.upload.removeEventListener("progress", this.progress);
        this.xmlHTTP.removeEventListener("error", this.error);
        this.xmlHTTP.upload.removeEventListener("abort", this.abort);
        this.xmlHTTP.onreadystatechange = null;
        this.xmlHTTP = null;

        this.reader.removeEventListener('loadstart', this.loadStartReader);
        this.reader.removeEventListener('load', this.loadCompleteReader);
        this.reader.removeEventListener('loadend', this.loadEndReader);
        this.reader.removeEventListener('progress', this.progressReader);
        this.reader.removeEventListener('error', this.errorReader);
        this.reader.removeEventListener('abort', this.abortReader);
        this.reader = null;
    }

    loadStartReader = (loadStart) => {
    }

    loadCompleteReader = (load) => {
    }

    loadEndReader = (loadEnd) => {
    }

    progressReader = (progress) => {
    }

    errorReader = (error) => {
        console.log(error)
    }

    abortReader = (abort) => {
        console.log(abort)
    }

    progress = (event) => {
        const percentage = parseInt((event.loaded / event.total) * 100);
            this.setState({
                percent: percentage
            });
    }

    error = (error) => {
        this.setState({
            fileProgress: true
        });
        console.log(error)
    }

    abort = (abort) => {
        console.log(abort)
        this.setState({
            fileProgress: true,
            percent: 0
        });
    }

    transferComplete = () => {
        this.saveData(this.state.file);
    }

    handleFileType = (idx, idxState) => {
        this.setState({
            [idxState]: idx
        })
    }

    handleSelectFiles = () => {
        const { handleFileType } = this;
        const { fileIdx, filesTypes } = this.state;
        return(
            <div style={{padding: "20px"}}>
               <CustomSelect 
                id={"fileType"} 
                labelId={"FileType-label"} 
                label={"file"}
                idx={fileIdx}
                idxState={"fileIdx"}
                fieldToShow={"file"}
                idItem={"fileType"}
                items={filesTypes}
                handleChange={handleFileType}
                /> 
            </div>
            
        )
    }

    handleSave = async() => {
        const { filesTemp } = this.state;
        this.setState({
            fileProgress: false
        })
         if (this.state.editFiles=== add) {
            var data;
            var fileBlob = filesTemp[0]
            this.reader.onloadend = (function (theFile) {
                return function (e) {
                    data = e.target.result;
                };
            })(fileBlob);

            this.reader.readAsArrayBuffer(fileBlob);
            var file = filesTemp[0];
            var ext = this.getExtension(file.name);
            var name = this.getFilename(file.name);
            file.blobName = uuidv4() + "." + ext;
            file.fileExtension = ext;
            file.mimeType = file.type;
            file.filename = name;
            file.fileType = constants.filesTypes[this.state.fileIdx].file;
            file.fileSize = file.size;
            this.setState({
                file: file
            }, async() => {
                var model = await this.getBlobToken(this.state.file.blobName, permissionFile.Write, this.state.file.size);
                if (model.flag) {
                    this.sendFile(model.sasToken, data);
                }
            })
        }
        else {
            var temp = this.state.file;
            temp.fileType = constants.filesTypes[this.state.fileIdx].file;
            this.setState({
                file: temp
            })
            this.saveData()
        }
    }

    getBlobToken = async(blobName, permission, size) => {
        var flag = false;
        var sasToken = await blobsApi.GetSASToken(blobName, permission, size)
        .then((result) =>  {
            flag = true;
            return result;
        })
        .catch((error) => {
            console.log(error)
        })
        return {
            flag: flag,
            sasToken: sasToken
        }
    }

    getFilename = (filename) => {
        var idx = filename.lastIndexOf(".");
        if(idx === -1){
            return filename
        }
        return filename.substring(0, idx);
    }

    getExtension(filename) {
        return filename.slice((filename.lastIndexOf('.') - 1 >>> 0) + 2);
    }
    
    onDrop =(accepted) => {
        this.setState({
            filesTemp: accepted,
            porcent: 0
        })
    }

    renderPreviews = () => {
        const { classes } = this.props;
        const { filesTemp, editFiles } = this.state;
        return(
            <CustomImage 
                url={filesTemp.length !== 0 ? URL.createObjectURL(filesTemp[0]) : editFiles.url} 
                topPreview={classes.preview}
                bottomPreview={classes.previewInner}
                image={classes.img}
            />
        )
    }

    saveData = async() => {
        const { userAuth, insertFile, updateFile } = this.props;
        var flag = false;
        var item = {
            id: "",
            filename: this.state.file.filename,
            fileExtension: this.state.file.fileExtension,
            fileType: this.state.file.fileType,
        }

        var model = {
            container: userAuth.id,
            blobName: this.state.file.blobName,
            fileExtension: this.state.file.fileExtension,
            mimeType: this.state.file.type,
            filename: this.state.file.filename,
            fileType: this.state.file.fileType,
            blobType: constants.checkBlobTypes[this.state.file.fileExtension],
            fileSize: this.state.file.fileSize
        }

        if (this.state.editFiles === add) {
            item.url =  URL.createObjectURL(this.state.file);
            model.mimeType = this.state.file.type;
        }
        else{
            item.url = this.state.editFiles.url;
            model.mimeType = this.state.editFiles.mimeType;
        }

        if (this.state.editFiles === add) {
            await blobsApi.CreateBlob(model)
            .then( async(result) => {
                item.id = result;
                flag = true
            })
            .catch((error) => {
                console.log(error)
            })
            if(flag) {
                insertFile(item);   
            }
        }
        else {
            model.id = this.state.editFiles.id;
            await blobsApi.UpdateBlob(model)
            .then((result) => {
                flag = true;
                item.id = this.state.editFiles.id;
            })
            .catch((error) => {
                console.log(error)
            })
            if(flag) {
                updateFile(item);  
            }
        }
    }


    sendFile = (sasToken, data) => {
        var idx = sasToken.uri.indexOf("?")
        var url = sasToken.uri.substring(0, idx) + "/" + sasToken.container + "/" + sasToken.blobName + sasToken.token;
        this.xmlHTTP.open("PUT", url, true);
        this.xmlHTTP.setRequestHeader("x-ms-blob-type", "BlockBlob");
        this.xmlHTTP.setRequestHeader('Content-Type', this.state.file.type);
        this.xmlHTTP.send(data);
    }

    render(){
        const { classes } = this.props;
        const { filesTemp, percent, editFiles, fileProgress } = this.state;
        const { onDrop, handleSave, handleSelectFiles, renderPreviews, handleCloseModal, handleAbortFile} = this;
        return(
            <Fragment>
            <div className={classes.closeIcon}>
                <CustomIcon 
                    Icon={icons.icon.CloseIcon}
                    fontSize={"large"}
                    handleClick={handleCloseModal}
                />
            </div> 
            <div  className={classes.dragAndDrop}>
                <Dropzone 
                    onDrop={onDrop} accept={".jpg, .png, .pdf"}
                    multiple={false}
                >
                    {({getRootProps, getInputProps}) => (
                        <section className="container"> 
                            <div {...getRootProps()}>
                                <input {...getInputProps()} />
                                {renderPreviews()}
                                <div>
                                    <CustomIcon Icon={icons.icon.CloudUploadIcon} fontSize={"large"} />
                                </div>
                                <p className={classes.typography} ></p>
                            </div>
                                {handleSelectFiles()}
                            {fileProgress !== true  && 
                                <div >
                                    <div style={{padding: "24px"}} >
                                        <LinearProgress variant="determinate" value={percent} />
                                    </div>
                                    <div style={{padding: "24px"}} >
                                        <CustomButton 
                                            content={"Abort"}
                                            color={"secondary"}
                                            handleClick={handleAbortFile}
                                        />
                                    </div>
                                </div>
                            }
                            <div>
                                <CustomIcon 
                                    Icon={icons.icon.SaveIcon} 
                                    fontSize={"large"} 
                                    disabled={filesTemp.length === 0 && editFiles === add } 
                                    handleClick={handleSave} 
                                    color={filesTemp.length === 0 & editFiles === add ? "disabled" : "primary" }
                                    newClass={classes.closeIcon}
                                />
                            </div>
                            
                        </section>
                    )}
                </Dropzone>
          </div>
          </Fragment>
        )
    }
}


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

export default compose(
    withRouter,
    withStyles(useStyles, { withTheme: true }),
    connect(mapStateToProps, mapDispatchToProps)
    )(CustomDragAndDrop);