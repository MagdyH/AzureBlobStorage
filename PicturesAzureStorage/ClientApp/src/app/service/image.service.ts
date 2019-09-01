import { Injectable,Inject } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, catchError, tap } from 'rxjs/operators';
import { Blob,FileUpload } from '../model/blob';

@Injectable({
  providedIn: 'root'
})
/*
angular service which communicate with back-end APIs
*/
export class ImageService {

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  // adding opyion for requests.
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type':  'application/json'
    })
  };


  // Rest Items Service: Read all REST Items
  getAll() {
    return this.http
      .get<Blob[]>(this.baseUrl+ 'api/AzureStorage/GetImagesBlob')
      .pipe(map(data => data));
  }

  // download service
  download(name:string): Observable<any> {
    return this.http
      .post<any>(this.baseUrl+ 'api/AzureStorage/'+name+'/DownloadBlobsFromContainer',null,this.httpOptions)
      .pipe(data => data);
  }

  // upload blob service
  upload (file:FileUpload) {
    return this.http
      .post<any>(this.baseUrl+ 'api/AzureStorage/UploadBlobsToContainer',JSON.stringify(file),this.httpOptions)
      .pipe(map(data => data));     
  }

  // delete blob service
  delete (name:string) {
    return this.http.delete<any>(this.baseUrl+ 'api/AzureStorage/'+name+'/DeleteBlobsFromContainer',this.httpOptions)
    .pipe(map(data => data));
  }
  
  // delete all blobs
  deleteAll (): Observable<any> {
    return this.http.delete<any>(this.baseUrl+ 'api/PrivateAzureStorage/DeleteAllBlobsFromContainer',this.httpOptions)
    .pipe(data => data);
  }

}
