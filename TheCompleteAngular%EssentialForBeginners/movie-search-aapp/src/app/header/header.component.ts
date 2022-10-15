import { Component, EventEmitter, OnInit, Output } from '@angular/core';

@Component({
  selector: 'Header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }
  @Output() startNewSearch = new EventEmitter()
  
}
