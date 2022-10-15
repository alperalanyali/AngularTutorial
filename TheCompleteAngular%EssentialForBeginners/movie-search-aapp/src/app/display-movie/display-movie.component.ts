import { Component, Input, OnInit } from '@angular/core';

import  {Movie} from'../Movie'

@Component({
  selector: 'display-movie',
  templateUrl: './display-movie.component.html',
  styleUrls: ['./display-movie.component.css']
})
export class DisplayMovieComponent implements OnInit {
@Input() movie:Movie = {}
  constructor() { }

  ngOnInit(): void {
  }

  getPosterUrl(path:string){
    return "http://cdn.collider.com/wp-content/uploads/the-avengers-robert-downey-jr-iron-man-poster.jpg"
  }

}
