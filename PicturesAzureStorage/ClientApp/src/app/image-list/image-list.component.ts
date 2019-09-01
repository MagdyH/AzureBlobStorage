import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { ImageService } from '../service/image.service';
import { Observable, of } from 'rxjs';
import { Blob, FileUpload } from '../model/blob';

@Component({
  selector: 'app-image-list',
  templateUrl: './image-list.component.html',
  styleUrls: ['./image-list.component.css']
})
/*
  the main component which have list,upload and delete blobs
*/
export class ImageListComponent implements OnInit {
  public blobs: Blob[];
  public filePath: string;
  public file: FileUpload = { fileName: '', fileSize: 0, fileType: '', fileBytes: '' };

  @ViewChild('downloadLink') private downloadLink: ElementRef;
  constructor(public service: ImageService,/*private _FileSaverService: FileSaverService*/) { }

  ngOnInit() {
    this.getBlobs();
  }

  //get all blobs
  getBlobs() {
    this.service.getAll().subscribe(result => {
      this.blobs = result;
    }, error => console.error(error))
  }

  //this is to convert uploaded blob to array of bytes
  readUploadedFile = (file: any): Observable<any> => {
    return new Observable(observer => {
      const reader = new FileReader();
      reader.onload = () => {
        this.file.fileBytes = reader.result.toString().split(',')[1];
        observer.next(this.file);
        observer.complete();
      };

      reader.readAsDataURL(file);
    });
  }

  // upload blob
  upload = (e: any) => {
    var files = e.target.files;
    var file = files[0];
    this.file.fileName = file.name;
    this.file.fileSize = file.size;
    this.file.fileType = file.type;
    return this.readUploadedFile(file).pipe().subscribe((result) => {
      this.service.upload(this.file)
        .subscribe(data => {
          this.blobs = data;
        });
    }
    );
  }

  //delete blob
  delete(name:string){
    this.service.delete(name)
        .subscribe(result => {
          this.blobs = result;         
        });
  }

  // delete all blobs
  deleteAll(){
    this.service.deleteAll()
        .subscribe(event => {
          this.blobs = [];
        });
  }
}
