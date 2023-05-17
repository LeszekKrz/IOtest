import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubscriptionsVideosComponent } from './subscriptions-videos.component';

describe('SubscriptionsComponent', () => {
  let component: SubscriptionsVideosComponent;
  let fixture: ComponentFixture<SubscriptionsVideosComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SubscriptionsVideosComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SubscriptionsVideosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
