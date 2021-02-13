export const add = "new";
export const minSizeWidthScreen = 640;

export const NotFound = "NotFound";
export const NotAuthorized = "NotAuthorized";
export const Found = "Found";
export const SecurityTokenExpired = "SecurityTokenExpired";
export const NotValidEmailConfirmation = "NotValidEmailConfirmation"
export const ValidateTokenConfirmEmailExpired = "ValidateTokenConfirmEmailExpired"
export const BarracudaSesion = "BarracudaSesion"
export const Block = "Block"
export const BOAAdmin = "BOAAdmin" //reserved word exclusive Barracuda for SuperAdmin Scope.

export const permissionFile = {
    Read: "Read",
    Write: "Write",
    Delete: "Delete"
};
export const filesTypes = [
    {
        id: 1,
        file: "INE"
    },
    {
        id: 2,
        file: "Insurance"
    },
    {
        id: 3,
        file: "RFC"
    },
    {
        id: 4,
        file: "Licence"
    },
    {
        id: 5,
        file: "Photo"
    }
];
export const BlobTypes = {
    File: "File",
    Image: "Image",
    Video: "Video",
    Audio: "Audio"
};
export const checkBlobTypes = {
    jpg: BlobTypes.Image,
    png: BlobTypes.Image,
    pdf: BlobTypes.File
};

export const UserAuth = "UserAuth";
