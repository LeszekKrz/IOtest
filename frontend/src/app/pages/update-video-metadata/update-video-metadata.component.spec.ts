import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateVideoMetadataComponent } from './update-video-metadata.component';

describe('UpdateVideoMetadataComponent', () => {
  let component: UpdateVideoMetadataComponent;
  let fixture: ComponentFixture<UpdateVideoMetadataComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UpdateVideoMetadataComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UpdateVideoMetadataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
