import { Component } from '@angular/core';
import { AuthStateProviderService } from '../services/auth-state-provider.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  constructor(private provider: AuthStateProviderService) { }

  isLoggedIn() {
    return this.provider.isAuthorized()
  }
}
