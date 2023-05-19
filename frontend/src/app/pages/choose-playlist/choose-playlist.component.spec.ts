import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChoosePlaylistComponent } from './choose-playlist.component';

describe('ChoosePlaylistComponent', () => {
  let component: ChoosePlaylistComponent;
  let fixture: ComponentFixture<ChoosePlaylistComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChoosePlaylistComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChoosePlaylistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
