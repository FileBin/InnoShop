import { StaticProvider, enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';
import { hosts } from './environments/hosts';

export function getBaseUrl() {
  return document.getElementsByTagName('base')[0].href;
}

export function getUserApiUrl() {
  return `${getBaseUrl()}api/accounts`;
}

const providers : StaticProvider[] = [
  { provide: 'BASE_URL', useFactory: getBaseUrl, deps: [] },
  { provide: 'USER_API_URL',  useFactory: getUserApiUrl },
  { provide: 'PRODUCT_API_URL',  useValue: hosts.productApi },
];

if (environment.production) {
  enableProdMode();
}

platformBrowserDynamic(providers).bootstrapModule(AppModule)
  .catch(err => console.log(err));
