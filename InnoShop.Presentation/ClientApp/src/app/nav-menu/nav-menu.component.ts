import { Component } from '@angular/core';
import { AuthStateProviderService } from '../services/auth-state-provider.service';
import { GlobalStateService } from '../services/global-state.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  filtersExpanded = false;

  constructor(private authStateProvider: AuthStateProviderService,
              private state: GlobalStateService) {}

  toggleFilters() {
    this.filtersExpanded = !this.filtersExpanded;
    this.state.changeFiltersState(this.filtersExpanded);
  }

  loggedIn() : boolean {
    return this.authStateProvider.isAuthorized();
  }
}
