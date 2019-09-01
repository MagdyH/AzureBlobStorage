export interface Blob {
    name:string,
    isDeleted:boolean,
    uri:string
}

export interface FileUpload {
    fileName:string,
    fileSize:number,
    fileType:string,
    fileBytes:string
}
