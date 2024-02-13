import { Component, Inject, OnInit, Renderer2 } from '@angular/core';
import { GlobalStateService } from '../services/global-state.service';
import { DOCUMENT } from '@angular/common';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent {
  expandSidebar = false;
  numbers: number[] = []
  constructor(private state: GlobalStateService) {
    this.numbers = Array(24).fill(0).map((x,i)=>i+1);

    state.filterExpand$.subscribe({
      next: x => this.expandSidebar = x
    })
  }
}
