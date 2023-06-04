import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, Validators } from '@angular/forms';
import { FileUpload } from 'primeng/fileupload';
import { Observable, Subscription, finalize, of, switchMap, tap } from 'rxjs';
import { descriptionMaxLength, titleMaxLength, visibilityOptions } from 'src/app/core/constants/video-constants';
import { UpdateVideoMetadataService } from './services/update-video-metadata.service';
import { VideoService } from 'src/app/core/services/video.service';
import { ActivatedRoute, Router } from '@angular/router';
import { VideoMetadataDto } from 'src/app/core/models/video-metadata-dto';
import { VideoMetadataUpdateDTO } from '../add-video/models/video-metadata-dto';

@Component({
  selector: 'app-update-video-metadata',
  templateUrl: './update-video-metadata.component.html',
  styleUrls: ['./update-video-metadata.component.scss']
})
export class UpdateVideoMetadataComponent implements OnInit, OnDestroy {
  videoId: string;
  titleMaxLength = titleMaxLength;
  descriptionMaxLength = descriptionMaxLength;
  visibilityOptions = visibilityOptions;
  updateVideoMetadataForm = new FormGroup({
    title: new FormControl('', [Validators.required, Validators.maxLength(this.titleMaxLength)]),
    description: new FormControl('', [Validators.required, Validators.maxLength(this.descriptionMaxLength)]),
    thumbnail: new FormControl('', Validators.required),
    tags: new FormControl<string[]>([]),
    visibility: new FormControl(this.visibilityOptions[0], Validators.required),
  });
  @ViewChild('thumbnailUpload') thumbnailUpload!: FileUpload;
  isProgressSpinnerVisible = false;
  subscriptions: Subscription[] = [];

  constructor(
    private updateVideoMetadataService: UpdateVideoMetadataService,
    private videoService: VideoService,
    private router: Router,
    private route: ActivatedRoute) {
      this.videoId = this.route.snapshot.params['videoId'];
  }

  ngOnInit(): void {
    const getVideoMetadata$ = this.videoService.getVideoMetadata(this.videoId).pipe(
      switchMap((videoMetadataDTO: VideoMetadataDto) => {
        this.updateVideoMetadataForm.patchValue({
          title: videoMetadataDTO.title,
          description: videoMetadataDTO.description,
          tags: videoMetadataDTO.tags,
          visibility: videoMetadataDTO.visibility
        });
        return this.updateVideoMetadataService.downloadFileImage(videoMetadataDTO.thumbnail).pipe(
          tap((blob: Blob) => {
            const file = new File([blob], 'thumbnail', { type: blob.type });

            let fileList: FileList = {
              0: file,
              length: 1,
              item: (_: number) => file,
            };

            let event = {
              type: 'change',
              target: {
                files: fileList,
              },
            };
            this.thumbnailUpload.onFileSelect(event);
          }),
        );
      }),
    );
    this.subscriptions.push(this.doWithLoading(getVideoMetadata$).subscribe());
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => {
      subscription.unsubscribe();
    });
  }

  onSubmit(): void {
    if (this.updateVideoMetadataForm.invalid) {
      this.updateVideoMetadataForm.markAllAsTouched();
      return;
    }

    const videoMedatadaDTO: VideoMetadataUpdateDTO = {
      title: this.updateVideoMetadataForm.get('title')!.value!,
      description: this.updateVideoMetadataForm.get('description')!.value!,
      thumbnail: this.updateVideoMetadataForm.get('thumbnail')!.value!,
      tags: this.updateVideoMetadataForm.get('tags')!.value!,
      visibility: this.updateVideoMetadataForm.get('visibility')!.value!,
    };

    const uploadVideo$ = this.updateVideoMetadataService.updateVideoMetadata(videoMedatadaDTO, this.videoId);
    this.subscriptions.push(this.doWithLoading(uploadVideo$).subscribe({
      complete: () => {
        this.router.navigate(['videos/' + this.videoId]);
      }
    }));
  }

  isInputInvalidAndTouchedOrDirty(inputName: string): boolean {
    const control = this.updateVideoMetadataForm.get(inputName)!;
    return this.isInputTouchedOrDirty(control) && control.invalid;
  }

  isInputTouchedOrDirtyAndEmpty(inputName: string): boolean {
    const control = this.updateVideoMetadataForm.get(inputName)!;
    return this.isInputTouchedOrDirty(control) && control.hasError('required');
  }

  isInputTouchedOrDirtyAndTooLong(inputName: string): boolean {
    const control = this.updateVideoMetadataForm.get(inputName)!;
    return this.isInputTouchedOrDirty(control) && control.hasError('maxlength');
  }

  private isInputTouchedOrDirty(control: AbstractControl): boolean {
    return control.touched || control.dirty;
  }

  handleOnThumbnailSelect(event: { originalEvent: Event; files: File[] }): void {
    const thumbnailFile = event.files[0];
    if (thumbnailFile.type === 'image/png' || thumbnailFile.type === 'image/jpeg') {
      const reader = new FileReader();
      reader.readAsDataURL(thumbnailFile);
      reader.onload = () => {
        this.updateVideoMetadataForm.patchValue({thumbnail: reader.result as string});
      };
    }
  }

  handleOnThumbnailRemove(): void {
    this.updateVideoMetadataForm.patchValue({thumbnail: ''});
    this.thumbnailUpload.clear();
  }

  private doWithLoading(observable$: Observable<any>): Observable<any> {
    return of(this.isProgressSpinnerVisible = true).pipe(
      switchMap(() => observable$),
      finalize(() => this.isProgressSpinnerVisible = false)
    );
  }
}
