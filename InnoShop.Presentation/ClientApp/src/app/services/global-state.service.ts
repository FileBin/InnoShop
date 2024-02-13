import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class GlobalStateService {
  public filterExpand$ = new Subject<boolean>();

  constructor() { }

  public changeFiltersState(count: boolean) {
    this.filterExpand$.next(count);
  }
}
