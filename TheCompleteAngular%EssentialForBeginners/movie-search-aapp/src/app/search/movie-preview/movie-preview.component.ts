import { Component, Input, OnInit } from '@angular/core';

import { Movie } from 'src/app/Movie';

@Component({
  selector: 'movie-preview',
  templateUrl: './movie-preview.component.html',
  styleUrls: ['./movie-preview.component.css']
})
export class MoviePreviewComponent implements OnInit {
  @Input()  movie:Movie = {}
  constructor() { }

  ngOnInit(): void {
  }

}
