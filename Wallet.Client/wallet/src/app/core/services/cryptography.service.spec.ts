import { TestBed, inject } from '@angular/core/testing';

import { CryptographyService } from './cryptography.service';

describe('CryptographyService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [CryptographyService]
    });
  });

  it('should be created', inject([CryptographyService], (service: CryptographyService) => {
    expect(service).toBeTruthy();
  }));
});
