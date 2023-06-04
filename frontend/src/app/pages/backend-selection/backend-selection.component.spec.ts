import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BackendSelectionComponent } from './backend-selection.component';

describe('BackendSelectionComponent', () => {
  let component: BackendSelectionComponent;
  let fixture: ComponentFixture<BackendSelectionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BackendSelectionComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BackendSelectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
