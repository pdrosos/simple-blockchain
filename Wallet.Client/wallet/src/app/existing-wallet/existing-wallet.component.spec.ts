import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExistingWalletComponent } from './existing-wallet.component';

describe('ExistingWalletComponent', () => {
  let component: ExistingWalletComponent;
  let fixture: ComponentFixture<ExistingWalletComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExistingWalletComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExistingWalletComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
