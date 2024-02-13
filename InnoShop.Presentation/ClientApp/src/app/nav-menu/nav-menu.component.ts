import { Component } from '@angular/core';
import { AuthStateProviderService } from '../services/auth-state-provider.service';
import { GlobalStateService } from '../services/global-state.service';
import { NavigationEnd, Router } from '@angular/router';
import { filter, map, startWith } from 'rxjs';
import { SearchStateService } from '../services/search-state.service';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  filtersExpanded = false;
  showFilters = false;

  searchLine = new FormControl('');

  constructor(private authStateProvider: AuthStateProviderService,
    private state: GlobalStateService,
    private router: Router,
    private searchState: SearchStateService) {
    router.events.subscribe((val) => {
      if (val instanceof NavigationEnd) {
        this.showFilters = (val.url == '/');
      }
    });

    this.searchLine.valueChanges.subscribe(x => this.setSearchLine(x));
  }

  toggleFilters() {
    this.filtersExpanded = !this.filtersExpanded;
    this.state.changeFiltersState(this.filtersExpanded);
  }

  loggedIn(): boolean {
    return this.authStateProvider.isAuthorized();
  }

  setSearchLine(x: string | null) {
    if (x === null) return;
    var filters = this.searchState.filters$.value;
    filters.contains = x;
    this.searchState.filters$.next(filters);
  }

  search() {
    this.searchState.search();
  }
}
