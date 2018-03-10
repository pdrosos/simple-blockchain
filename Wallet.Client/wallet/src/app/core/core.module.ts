import { NgModule, Optional, SkipSelf } from '@angular/core';

import { WalletService } from './services/wallet.service';
import { CryptographyService } from './services/cryptography.service';
import { ErrorHandlerService } from './services/error-handler.service';

@NgModule({
  imports: [],
  providers: [
    WalletService,
    CryptographyService,
    ErrorHandlerService
  ]
})
export class CoreModule {
  constructor (@Optional() @SkipSelf() parentModule: CoreModule) {
      if (parentModule) {
          throw new Error('CoreModule is already loaded. Import it in the AppModule only');
      }
  }
}
