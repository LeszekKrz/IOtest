import { TestBed } from '@angular/core/testing';

import { UpdateVideoMetadataService } from './update-video-metadata.service';

describe('UpdateVideoMetadataServiceService', () => {
  let service: UpdateVideoMetadataService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UpdateVideoMetadataService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
